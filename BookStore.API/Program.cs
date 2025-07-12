using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text;
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
    
    // Enhanced debugging for Docker/Render environment
    Log.Information("=== Database Configuration Debug ===");
    Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);
    Log.Information("Is Production: {IsProduction}", builder.Environment.IsProduction());
    Log.Information("DATABASE_URL exists: {HasDatabaseUrl}", databaseUrl != null);
    Log.Information("DefaultConnection exists: {HasDefaultConnection}", connectionString != null);
    
    // Log all environment variables starting with DATABASE for debugging
    var dbEnvVars = Environment.GetEnvironmentVariables()
        .Cast<System.Collections.DictionaryEntry>()
        .Where(e => e.Key?.ToString()?.StartsWith("DATABASE", StringComparison.OrdinalIgnoreCase) == true)
        .ToDictionary(e => e.Key?.ToString() ?? "unknown", e => e.Value?.ToString() ?? "null");
    
    Log.Information("Database-related environment variables count: {Count}", dbEnvVars.Count);
    foreach (var kvp in dbEnvVars)
    {
        var safeValue = kvp.Value?.Length > 20 ? kvp.Value.Substring(0, 20) + "..." : kvp.Value ?? "null";
        Log.Information("  {Key}: {Value}", kvp.Key, safeValue);
    }
    
    // Log the connection string source for debugging (without sensitive data)
    if (databaseUrl != null)
    {
        Log.Information("Connection string source: DATABASE_URL environment variable");
        Log.Information("DATABASE_URL format: {Format}", 
            databaseUrl.StartsWith("postgres://") ? "postgres://" : 
            databaseUrl.StartsWith("postgresql://") ? "postgresql://" : "unknown");
        Log.Information("DATABASE_URL length: {Length}", databaseUrl.Length);
        Log.Information("DATABASE_URL first 50 chars: {FirstChars}", 
            databaseUrl.Length > 50 ? databaseUrl.Substring(0, 50) + "..." : databaseUrl);
        
        // Check for common issues
        if (string.IsNullOrWhiteSpace(databaseUrl))
        {
            Log.Error("DATABASE_URL is empty or whitespace!");
        }
        else if (databaseUrl.StartsWith(" ") || databaseUrl.EndsWith(" "))
        {
            Log.Error("DATABASE_URL has leading or trailing spaces!");
        }
        else if (!databaseUrl.StartsWith("postgresql://") && !databaseUrl.StartsWith("postgres://"))
        {
            Log.Error("DATABASE_URL does not start with postgresql:// or postgres://");
        }
    }
    else if (connectionString != null)
    {
        Log.Information("Connection string source: {Source}", 
            connectionString.Substring(0, Math.Min(20, connectionString.Length)) + "...");
    }
    else
    {
        Log.Information("Connection string source: null");
    }
    
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
            finalConnectionString = databaseUrl;
            Log.Information("Using DATABASE_URL for PostgreSQL connection");
        }
        else if (connectionString != null)
        {
            // If connection string contains SQL Server specific settings, create a basic PostgreSQL connection string
            if (connectionString.Contains("Server=(localdb)", StringComparison.OrdinalIgnoreCase) ||
                connectionString.Contains("trusted_connection", StringComparison.OrdinalIgnoreCase) ||
                connectionString.Contains("Integrated Security", StringComparison.OrdinalIgnoreCase))
            {
                // This is a SQL Server connection string, but we need PostgreSQL
                Log.Warning("SQL Server connection string detected but PostgreSQL is required. Using fallback connection string.");
                
                // In production, we should fail if DATABASE_URL is not set
                if (builder.Environment.IsProduction())
                {
                    throw new InvalidOperationException("DATABASE_URL environment variable is required for PostgreSQL in production environment. Please ensure your database service is properly configured.");
                }
                else
                {
                    // Use fallback for development/testing
                    finalConnectionString = "Host=localhost;Database=BookStore;Username=postgres;Password=postgres;";
                    Log.Warning("Using fallback PostgreSQL connection string for development.");
                }
            }
            else
            {
                finalConnectionString = connectionString;
                Log.Information("Using connection string for PostgreSQL connection");
            }
        }
        else
        {
            // No connection string available
            if (builder.Environment.IsProduction())
            {
                throw new InvalidOperationException("No database connection string available. DATABASE_URL environment variable is required for production.");
            }
            else
            {
                finalConnectionString = "Host=localhost;Database=BookStore;Username=postgres;Password=postgres;";
                Log.Warning("No connection string found. Using default PostgreSQL connection for development.");
            }
        }
    }
    
    if (usePostgreSQL)
    {
        // Use PostgreSQL for cloud deployments
        Log.Information("Using PostgreSQL database");
        Log.Information("Final PostgreSQL connection string first 50 chars: {ConnectionString}", 
            finalConnectionString.Length > 50 ? finalConnectionString.Substring(0, 50) + "..." : finalConnectionString);
        
        // Validate connection string format before using it
        if (string.IsNullOrWhiteSpace(finalConnectionString))
        {
            throw new InvalidOperationException("PostgreSQL connection string is null or empty");
        }
        
        try
        {
            // Test the connection string format by creating a NpgsqlConnectionStringBuilder
            var testBuilder = new Npgsql.NpgsqlConnectionStringBuilder(finalConnectionString);
            Log.Information("Connection string validation successful - Host: {Host}, Database: {Database}", 
                testBuilder.Host, testBuilder.Database);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Connection string validation failed: {ConnectionString}", finalConnectionString);
            throw new InvalidOperationException($"Invalid PostgreSQL connection string format: {ex.Message}");
        }
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(finalConnectionString));
    }
    else
    {
        // Use SQL Server for local development
        Log.Information("Using SQL Server database");
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
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true
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
    options.ExcludedHosts.Clear(); // Remove localhost from excluded hosts in development
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "https://localhost:5173",
                "https://localhost:5174", 
                "https://localhost:3000",
                "https://localhost:5001" // Allow API to call itself if needed
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

// Configure Kestrel to use custom SSL certificate
// Temporarily disabled custom certificate to use default development certificate
/*
if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.ConfigureHttpsDefaults(httpsOptions =>
        {
            var certificatePath = Path.Combine(builder.Environment.ContentRootPath, "..", "certs", "bookstore-dev.pfx");
            if (File.Exists(certificatePath))
            {
                try
                {
                    httpsOptions.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(
                        certificatePath, 
                        "bookstore123");
                    Log.Information("Using custom SSL certificate from {CertificatePath}", certificatePath);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to load custom SSL certificate from {CertificatePath}. Using default development certificate.", certificatePath);
                }
            }
            else
            {
                Log.Warning("Custom SSL certificate not found at {CertificatePath}. Using default development certificate.", certificatePath);
            }
        });
    });
}
*/

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

app.UseHttpsRedirection();

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
    const int maxRetries = 5;
    const int delaySeconds = 10;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Database operation attempt {Attempt} of {MaxRetries}", attempt, maxRetries);
            
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Test database connection first
            await context.Database.CanConnectAsync();
            logger.LogInformation("Database connection successful");
            
            // Apply pending migrations
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");

            // Seed initial data
            var seeder = new DataSeeder(context, userManager, roleManager);
            await seeder.SeedAsync();
            logger.LogInformation("Database seeding completed successfully");
            
            return; // Success - exit retry loop
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database operation failed on attempt {Attempt}: {Message}", attempt, ex.Message);
            
            if (attempt == maxRetries)
            {
                logger.LogError("All database operation attempts failed. Application will exit.");
                throw;
            }
            
            logger.LogInformation("Retrying database operation in {DelaySeconds} seconds...", delaySeconds);
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
