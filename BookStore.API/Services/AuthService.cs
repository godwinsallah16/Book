using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using AutoMapper;
using BookStore.API.DTOs;
using BookStore.API.Models;
using BookStore.API.Services.Interfaces;

namespace BookStore.API.Services
{
    public class AuthService : IAuthService
    {
        // In-memory store for refresh tokens (for demo; use DB for production)
        private static readonly Dictionary<string, (string RefreshToken, DateTime Expiration)> _refreshTokens = new();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            IMapper mapper,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration attempt for existing email: {Email}", registerDto.Email);
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorCode = "USER_EXISTS",
                        Errors = new List<string> { "A user with this email address already exists." }
                    };
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false // Require email verification
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("User registration failed for {Email}: {Errors}", 
                        registerDto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorCode = "VALIDATION_ERROR",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                // Assign User role to new user
                await _userManager.AddToRoleAsync(user, "User");

                // Send email verification
                await SendEmailVerificationAsync(user);

                // DO NOT generate JWT token - require email verification first
                _logger.LogInformation("User registered successfully: {Email} - Email verification required", registerDto.Email);

                return new AuthResultDto
                {
                    Success = true,
                    Data = new AuthResponseDto
                    {
                        Token = string.Empty, // No token until email verified
                        UserId = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Expiration = DateTime.MinValue, // No expiration since no token
                        EmailConfirmed = user.EmailConfirmed,
                        Roles = new List<string>() // No roles until verified
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration for {Email}", registerDto.Email);
                return new AuthResultDto
                {
                    Success = false,
                    ErrorCode = "INTERNAL_ERROR",
                    Errors = new List<string> { "An internal error occurred. Please try again later." }
                };
            }
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt for non-existent email: {Email}", loginDto.Email);
                    return null;
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login attempt for inactive user: {Email}", loginDto.Email);
                    return null;
                }

                // Allow login for unverified users. EmailConfirmed will be returned in response.

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed login attempt for {Email}", loginDto.Email);
                    return null;
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var token = await GenerateJwtTokenAsync(user.Email!, user.Id);
                var expiration = DateTime.UtcNow.AddHours(24);

                // Generate refresh token
                var refreshToken = await GenerateRefreshTokenAsync();
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);
                _refreshTokens[user.Id] = (refreshToken, refreshTokenExpiration);

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Expiration = expiration,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshTokenExpiration
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for {Email}", loginDto.Email);
                throw;
            }
        }

        public async Task<bool> VerifyEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Email verification attempt for non-existent user: {UserId}", userId);
                    return false;
                }

                var decodedToken = HttpUtility.UrlDecode(token);
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                
                if (result.Succeeded)
                {
                    user.EmailConfirmed = true;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    // Send welcome email
                    await _emailService.SendWelcomeEmailAsync(user.Email!, user.FirstName);

                    _logger.LogInformation("Email verified successfully for user: {Email}", user.Email);
                    return true;
                }

                _logger.LogWarning("Email verification failed for user: {UserId}", userId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during email verification for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    _logger.LogWarning("Password reset request for non-existent email: {Email}", email);
                    return true; // Return true to prevent user enumeration
                }

                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);

                // Create reset link
                var frontendUrl = _configuration["FrontendSettings:BaseUrl"];
                var resetLink = $"{frontendUrl}/reset-password?email={HttpUtility.UrlEncode(email)}&token={encodedToken}";

                // Send password reset email
                await _emailService.SendPasswordResetAsync(email, user.FirstName, resetLink);

                _logger.LogInformation("Password reset email sent to: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset request for: {Email}", email);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Password reset attempt for non-existent email: {Email}", resetPasswordDto.Email);
                    return false;
                }

                var decodedToken = HttpUtility.UrlDecode(resetPasswordDto.Token);
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.Password);

                if (result.Succeeded)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("Password reset successfully for user: {Email}", resetPasswordDto.Email);
                    return true;
                }

                _logger.LogWarning("Password reset failed for user: {Email}", resetPasswordDto.Email);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset for: {Email}", resetPasswordDto.Email);
                return false;
            }
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Resend verification email request for non-existent email: {Email}", email);
                    return false;
                }

                if (user.EmailConfirmed)
                {
                    _logger.LogWarning("Resend verification email request for already confirmed email: {Email}", email);
                    return false;
                }

                await SendEmailVerificationAsync(user);
                _logger.LogInformation("Verification email resent to: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during resend verification email for: {Email}", email);
                return false;
            }
        }

        private async Task SendEmailVerificationAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);

            // Get frontend URL from environment variable or fallback to config
            var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") 
                ?? _configuration["FrontendSettings:BaseUrl"];
            
            var verificationLink = $"{frontendUrl}/verify-email?userId={user.Id}&token={encodedToken}";

            await _emailService.SendEmailVerificationAsync(user.Email!, user.FirstName, verificationLink);
        }

        public async Task<string> GenerateJwtTokenAsync(string email, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("User not found", nameof(userId));
                }

                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secret = jwtSettings["Secret"];
                if (string.IsNullOrEmpty(secret))
                {
                    _logger.LogError("JWT Secret is null or empty! Check your environment variables and configuration.");
                }
                else
                {
                    _logger.LogInformation("JWT Secret loaded. Length: {Length}", secret.Length);
                }
                var key = Encoding.ASCII.GetBytes(secret!);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, 
                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                        ClaimValueTypes.Integer64)
                };

                // Add role claims
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpiryInHours"])),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating JWT token for {Email}", email);
                throw;
            }
        }

        public async Task<string> GenerateRefreshTokenAsync()
        {
            // Use a cryptographically secure random generator
            var randomBytes = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string userId, string refreshToken)
        {
            if (!_refreshTokens.TryGetValue(userId, out var stored) || stored.RefreshToken != refreshToken || stored.Expiration < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired refresh token for user: {UserId}", userId);
                return null;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Refresh token attempt for invalid or inactive user: {UserId}", userId);
                return null;
            }

            var token = await GenerateJwtTokenAsync(user.Email!, user.Id);
            var expiration = DateTime.UtcNow.AddHours(24);

            // Optionally, rotate refresh token
            var newRefreshToken = await GenerateRefreshTokenAsync();
            var newRefreshTokenExpiration = DateTime.UtcNow.AddDays(7);
            _refreshTokens[user.Id] = (newRefreshToken, newRefreshTokenExpiration);

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Expiration = expiration,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiration = newRefreshTokenExpiration
            };
        }
    }
}
