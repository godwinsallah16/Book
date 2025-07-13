using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text;
using System.Security.Claims;
using Serilog;
using FluentValidation.AspNetCore;
using Npgsql;
using BookStore.API.Data;
using BookStore.API.Models;
using BookStore.API.Services;
using BookStore.API.Services.Interfaces;
using BookStore.API.Repositories;
using BookStore.API.Repositories.Interfaces;
using BookStore.API.Mappings;
using BookStore.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for production
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/bookstore-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting BookStore API");

    // Add services to the container.
    if (builder.Environment.IsEnvironment("Testing"))
    {
        // Use InMemory database for testing
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));
    }
    else
    {
        // PostgreSQL configuration for cloud deployment
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        string connectionString;

        if (!string.IsNullOrEmpty(databaseUrl))
        {
            // Parse Render/Heroku style DATABASE_URL
            var uri = new Uri(databaseUrl);
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port == -1 ? 5432 : uri.Port,
                Database = uri.LocalPath.TrimStart('/'),
                Username = uri.UserInfo.Split(':')[0],
                Password = uri.UserInfo.Split(':')[1],
                SslMode = SslMode.Require
            };
            connectionString = connectionStringBuilder.ConnectionString;
            Log.Information("Using DATABASE_URL for PostgreSQL connection");
        }
        else
        {
            // Fallback to connection string from appsettings
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? "Host=localhost;Database=BookStore;Username=postgres;Password=postgres;";
            Log.Information("Using connection string from configuration");
        }

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    // Configure Identity for production
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password settings
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequiredUniqueChars = 4;
        
        // Email settings (disabled for cloud deployment without email service)
        options.SignIn.RequireConfirmedEmail = false;
        options.User.RequireUniqueEmail = true;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // Token settings
        options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // Configure JWT Authentication with environment variables
    var jwtSecret = Environment.GetEnvironmentVariable("JwtSettings__Secret") 
        ?? builder.Configuration["JwtSettings:Secret"] 
        ?? "ThisIsAVeryLongSecretKeyForJWTTokenGenerationThatShouldBeAtLeast256BitsLong";
    
    var jwtIssuer = Environment.GetEnvironmentVariable("JwtSettings__Issuer") 
        ?? builder.Configuration["JwtSettings:Issuer"] 
        ?? "BookStoreAPI";
    
    var jwtAudience = Environment.GetEnvironmentVariable("JwtSettings__Audience") 
        ?? builder.Configuration["JwtSettings:Audience"] 
        ?? "BookStoreClient";

    if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
    {
        throw new InvalidOperationException("JWT Secret must be at least 32 characters long");
    }

    var key = Encoding.ASCII.GetBytes(jwtSecret);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Disable for cloud deployment behind proxy
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true,
            RoleClaimType = ClaimTypes.Role
        };
    });

    // Add AutoMapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // Add FluentValidation
    builder.Services.AddFluentValidationAutoValidation();

    // Register repositories and services
    builder.Services.AddScoped<IBookRepository, BookRepository>();
    builder.Services.AddScoped<IBookService, BookService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IEmailService, EmailService>(); // Using real email service
    builder.Services.AddScoped<IFavoriteService, FavoriteService>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<IOrderService, OrderService>();

    // Configure CORS for production
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins(
                    "https://localhost:5173",
                    "https://localhost:3000",
                    "https://godwinsallah16.github.io", // GitHub Pages frontend
                    "https://bookstore-frontend-074u.onrender.com" // Render.com frontend
                )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    builder.Services.AddControllers();

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "BookStore API", 
            Version = "v1",
            Description = "A comprehensive Book Store Management API"
        });

        // Configure JWT authentication in Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    
    // Enable Swagger for development and testing
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API V1");
            if (app.Environment.IsProduction())
            {
                c.RoutePrefix = "swagger"; // Keep swagger at /swagger in production
            }
            else
            {
                c.RoutePrefix = string.Empty; // Serve swagger at root in development
            }
        });
    }

    // Add custom exception handling middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Configure forwarded headers for cloud deployment
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                          Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    });

    // Use CORS for production
    app.UseCors("AllowReactApp");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Add health check and API test endpoints
    app.MapGet("/health", () => "OK");
    app.MapGet("/", () => "BookStore API is running! Visit /swagger for API documentation.");

    // Database initialization for cloud deployment
    if (!builder.Environment.IsEnvironment("Testing"))
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Wait for database to be ready
            await WaitForDatabase(context, (Microsoft.Extensions.Logging.ILogger)logger);
            
            // Apply migrations
            if (context.Database.IsRelational())
            {
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                await context.Database.EnsureCreatedAsync();
            }

            // Seed initial data
            logger.LogInformation("Seeding initial data...");
            var seeder = new DataSeeder(context, userManager, roleManager);
            await seeder.SeedAsync();
            logger.LogInformation("Initial data seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize database");
            // Don't throw - let the app start anyway for debugging
        }
    }

    Log.Information("BookStore API initialization completed successfully");
    
    try
    {
        await app.RunAsync();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Exception occurred during application execution");
        throw;
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Helper method to wait for database
static async Task WaitForDatabase(ApplicationDbContext context, Microsoft.Extensions.Logging.ILogger logger)
{
    const int maxRetries = 10;
    const int delaySeconds = 3;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database (attempt {Attempt}/{MaxRetries})", attempt, maxRetries);
            await context.Database.CanConnectAsync();
            logger.LogInformation("Successfully connected to database");
            return;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database connection attempt {Attempt} failed", attempt);
            
            if (attempt == maxRetries)
            {
                logger.LogError("All database connection attempts failed");
                throw;
            }
            
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

// Make the Program class accessible for testing
public partial class Program { }
