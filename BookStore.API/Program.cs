using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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

Log.Information("🚀 BookStore API starting up...");
Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);
Log.Information("Content Root: {ContentRoot}", builder.Environment.ContentRootPath);

// Debug environment variables for troubleshooting
Log.Information("🔍 Environment Variables Check:");
Log.Information("DATABASE_URL: {DatabaseUrl}", Environment.GetEnvironmentVariable("DATABASE_URL")?.Substring(0, Math.Min(50, Environment.GetEnvironmentVariable("DATABASE_URL")?.Length ?? 0)) + "...");
Log.Information("JwtSettings__Secret: {JwtSecret}", string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JwtSettings__Secret")) ? "NOT SET" : "SET");
Log.Information("JwtSettings__Issuer: {JwtIssuer}", Environment.GetEnvironmentVariable("JwtSettings__Issuer") ?? "NOT SET");
Log.Information("JwtSettings__Audience: {JwtAudience}", Environment.GetEnvironmentVariable("JwtSettings__Audience") ?? "NOT SET");
Log.Information("ASPNETCORE_URLS: {AspNetCoreUrls}", Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "NOT SET");

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
    // Password settings
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredUniqueChars = 4;

    // Email settings
    options.SignIn.RequireConfirmedEmail = true; // Enable email confirmation
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
Log.Information("🔐 Configuring JWT Authentication...");
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtSecret = jwtSettings["Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    Log.Error("JWT Secret is not configured. Check JwtSettings__Secret environment variable.");
    throw new InvalidOperationException("JWT Secret is required but not configured");
}

var jwtIssuer = jwtSettings["Issuer"];
if (string.IsNullOrEmpty(jwtIssuer))
{
    Log.Error("JWT Issuer is not configured. Check JwtSettings__Issuer environment variable.");
    throw new InvalidOperationException("JWT Issuer is required but not configured");
}

var jwtAudience = jwtSettings["Audience"];
if (string.IsNullOrEmpty(jwtAudience))
{
    Log.Error("JWT Audience is not configured. Check JwtSettings__Audience environment variable.");
    throw new InvalidOperationException("JWT Audience is required but not configured");
}

var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // Always require HTTPS for JWT
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

// Add custom exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

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

// Create database and seed data (skip in test environment)
if (!builder.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
    await RetryDatabaseOperations(app.Services, logger);
}

async Task RetryDatabaseOperations(IServiceProvider services, Microsoft.Extensions.Logging.ILogger<Program> logger)
{
    const int maxRetries = 3;
    const int delaySeconds = 5;
    
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
                await context.Database.MigrateAsync();
            }
            else
            {
                // For InMemory databases, ensure the database is created
                await context.Database.EnsureCreatedAsync();
            }

            // Seed initial data
            var seeder = new DataSeeder(context, userManager, roleManager);
            await seeder.SeedAsync();
            
            return; // Success - exit retry loop
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database operation failed on attempt {Attempt}/{MaxRetries}. Error: {ErrorMessage}", 
                attempt, maxRetries, ex.Message);
            
            // Log connection string info (without sensitive data)
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var connectionString = context.Database.GetConnectionString();
            if (!string.IsNullOrEmpty(connectionString))
            {
                // Log sanitized connection string (remove password)
                var sanitized = connectionString.Contains("Password=") 
                    ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=****"
                    : connectionString;
                logger.LogError("Connection string: {ConnectionString}", sanitized);
            }
            
            if (attempt == maxRetries)
            {
                logger.LogCritical("All database operation attempts failed. Application will exit.");
                throw;
            }
            
            logger.LogWarning("Retrying in {DelaySeconds} seconds...", delaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

try
{
    Log.Information("✅ Configuration complete. Starting BookStore API server...");
    Log.Information("Server will listen on: {Urls}", Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "Default URLs");
    app.Run();
    Log.Information("📱 BookStore API started successfully and is ready to accept requests");
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
