using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/bookstore-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
if (builder.Environment.IsEnvironment("Testing"))
{
    // Use InMemory database for testing
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("TestDatabase"));
}
else
{
    // Check if we're using PostgreSQL (for cloud deployments like Render)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    
    // Determine if we should use PostgreSQL
    bool usePostgreSQL = false;
    string finalConnectionString = connectionString ?? "";
    
    // Check multiple indicators for PostgreSQL
    if (databaseUrl != null ||
        connectionString?.Contains("postgres", StringComparison.OrdinalIgnoreCase) == true ||
        connectionString?.Contains("postgresql", StringComparison.OrdinalIgnoreCase) == true ||
        connectionString?.Contains("npgsql", StringComparison.OrdinalIgnoreCase) == true ||
        connectionString?.Contains("Port=5432") == true ||
        builder.Environment.IsProduction()) // Assume PostgreSQL in production
    {
        usePostgreSQL = true;
        
        // Use DATABASE_URL if available, otherwise use the connection string
        if (databaseUrl != null)
        {
            // Convert PostgreSQL URL format to .NET connection string format
            if (databaseUrl.StartsWith("postgresql://") || databaseUrl.StartsWith("postgres://"))
            {
                try
                {
                    var uri = new Uri(databaseUrl);
                    var connectionStringBuilder = new NpgsqlConnectionStringBuilder
                    {
                        Host = uri.Host,
                        Port = uri.Port == -1 ? 5432 : uri.Port,
                        Database = uri.LocalPath.TrimStart('/'),
                        Username = uri.UserInfo.Split(':')[0],
                        Password = uri.UserInfo.Split(':')[1],
                        SslMode = SslMode.Require // Required for most cloud providers
                    };
                    
                    finalConnectionString = connectionStringBuilder.ConnectionString;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to parse DATABASE_URL");
                    throw new InvalidOperationException($"Invalid DATABASE_URL format: {ex.Message}");
                }
            }
            else
            {
                // Already in .NET connection string format
                finalConnectionString = databaseUrl;
            }
        }
        else if (connectionString != null)
        {
            // If connection string contains SQL Server specific settings, create a basic PostgreSQL connection string
            if (connectionString.Contains("Server=(localdb)", StringComparison.OrdinalIgnoreCase) ||
                connectionString.Contains("trusted_connection", StringComparison.OrdinalIgnoreCase) ||
                connectionString.Contains("Integrated Security", StringComparison.OrdinalIgnoreCase))
            {
                // This is a SQL Server connection string, but we need PostgreSQL
                if (builder.Environment.IsProduction())
                {
                    throw new InvalidOperationException("DATABASE_URL environment variable is required for PostgreSQL in production environment.");
                }
                else
                {
                    // Use fallback for development/testing
                    finalConnectionString = "Host=localhost;Database=BookStore;Username=postgres;Password=postgres;";
                }
            }
            else
            {
                finalConnectionString = connectionString;
            }
        }
        else
        {
            // No connection string available
            if (builder.Environment.IsProduction())
            {
                throw new InvalidOperationException("DATABASE_URL environment variable is required for production.");
            }
            else
            {
                finalConnectionString = "Host=localhost;Database=BookStore;Username=postgres;Password=postgres;";
            }
        }
    }
    
    if (usePostgreSQL)
    {
        // Use PostgreSQL for cloud deployments
        if (string.IsNullOrWhiteSpace(finalConnectionString))
        {
            throw new InvalidOperationException("PostgreSQL connection string is null or empty");
        }
        
        try
        {
            // Test the connection string format by creating a NpgsqlConnectionStringBuilder
            var testBuilder = new Npgsql.NpgsqlConnectionStringBuilder(finalConnectionString);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Connection string validation failed");
            throw new InvalidOperationException($"Invalid PostgreSQL connection string format: {ex.Message}");
        }
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(finalConnectionString));
    }
    else
    {
        // Use SQL Server for local development
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(finalConnectionString));
    }
}

// Configure Identity with enhanced security
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings - relaxed for cloud deployment
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredUniqueChars = 1;

    // Email settings - disabled for cloud deployment
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

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtSecret = Environment.GetEnvironmentVariable("JwtSettings__Secret") ?? jwtSettings["Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    Log.Warning("JWT Secret not found in environment variables, using default from appsettings");
    jwtSecret = "ThisIsAVeryLongSecretKeyForJWTTokenGenerationThatShouldBeAtLeast256BitsLong";
}

var jwtIssuer = Environment.GetEnvironmentVariable("JwtSettings__Issuer") ?? jwtSettings["Issuer"];
if (string.IsNullOrEmpty(jwtIssuer))
{
    Log.Warning("JWT Issuer not found, using default");
    jwtIssuer = "BookStoreAPI";
}

var jwtAudience = Environment.GetEnvironmentVariable("JwtSettings__Audience") ?? jwtSettings["Audience"];
if (string.IsNullOrEmpty(jwtAudience))
{
    Log.Warning("JWT Audience not found, using default");
    jwtAudience = "BookStoreClient";
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
        ClockSkew = TimeSpan.FromMinutes(5), // Allow some clock skew for cloud deployment
        RequireExpirationTime = true,
        RoleClaimType = ClaimTypes.Role // Ensure role claims are properly mapped
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
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Configure HTTPS with development certificates
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001;
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
});

// Configure HSTS (HTTP Strict Transport Security)
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
    options.ExcludedHosts.Clear();
});

// Add CORS
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

// Add security headers
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
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
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API V1");
    });
}
else
{
    // Enable Swagger in production for debugging on Render
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API V1");
        c.RoutePrefix = "swagger"; // Keep swagger at /swagger in production
    });
}

// Add custom exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure forwarded headers for cloud deployment (like Render)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Use HSTS in all environments to enforce HTTPS
app.UseHsts();

// Only use HTTPS redirection in development (Render.com handles HTTPS termination)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
    await next();
});

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoints for Render
app.MapGet("/health", () => "OK");
app.MapGet("/", () => "BookStore API is running! Visit /swagger for API documentation.");

// Create database and seed data (skip in test environment)
if (!builder.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
    
    try
    {
        await RetryDatabaseOperations(app.Services, logger);
        logger.LogInformation("Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed, but continuing with startup");
        // Don't throw - let the app start anyway for debugging
    }
}

async Task RetryDatabaseOperations(IServiceProvider services, Microsoft.Extensions.Logging.ILogger<Program> logger)
{
    const int maxRetries = 5;
    const int delaySeconds = 3;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            logger.LogInformation("Attempting database connection (attempt {Attempt}/{MaxRetries})", attempt, maxRetries);
            
            // Test database connection first
            await context.Database.CanConnectAsync();
            logger.LogInformation("Database connection successful");
            
            // Apply pending migrations (only for relational databases)
            if (context.Database.IsRelational())
            {
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                // For InMemory databases, ensure the database is created
                await context.Database.EnsureCreatedAsync();
            }

            // Seed initial data
            logger.LogInformation("Seeding initial data...");
            var seeder = new DataSeeder(context, userManager, roleManager);
            await seeder.SeedAsync();
            logger.LogInformation("Initial data seeded successfully");
            
            return; // Success - exit retry loop
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database operation failed on attempt {Attempt}/{MaxRetries}", attempt, maxRetries);
            
            if (attempt == maxRetries)
            {
                logger.LogError(ex, "All database operation attempts failed");
                throw;
            }
            
            logger.LogInformation("Waiting {DelaySeconds} seconds before retry...", delaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

try
{
    Log.Information("Starting BookStore API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make the Program class accessible for testing
public partial class Program { }
