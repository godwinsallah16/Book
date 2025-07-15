using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookStore.API.DTOs;
using BookStore.API.Services.Interfaces;
using BookStore.API.Models;
using System.Security.Claims;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>New access and refresh tokens</returns>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "UserId and RefreshToken are required" });
            }
            var result = await _authService.RefreshTokenAsync(request.UserId, request.RefreshToken);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }
            return Ok(result);
        }
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostEnvironment _hostEnvironment;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, UserManager<ApplicationUser> userManager, IHostEnvironment hostEnvironment)
        {
            _authService = authService;
            _logger = logger;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registerDto">User registration details</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", registerDto?.Email);
                
                if (registerDto == null)
                {
                    _logger.LogWarning("Registration failed: registerDto is null");
                    return BadRequest(new { 
                        success = false,
                        errorCode = "INVALID_REQUEST",
                        message = "Registration data is required",
                        errors = new[] { "Registration data is required" }
                    });
                }
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    _logger.LogWarning("Registration failed due to model validation errors for {Email}: {Errors}", 
                        registerDto.Email, string.Join(", ", errors));
                    
                    return BadRequest(new { 
                        success = false,
                        errorCode = "VALIDATION_ERROR",
                        message = "Validation failed. Please check your input and try again.",
                        errors = errors
                    });
                }

                var result = await _authService.RegisterAsync(registerDto);
                
                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errorCode = result.ErrorCode,
                        message = GetUserFriendlyMessage(result.ErrorCode),
                        errors = result.Errors
                    });
                }

                _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);
                return Ok(new
                {
                    success = true,
                    message = "Registration successful. Please check your email to verify your account.",
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration");
                return StatusCode(500, new { 
                    success = false,
                    errorCode = "INTERNAL_ERROR",
                    message = "An internal server error occurred during registration. Please try again later.",
                    errors = new[] { "An internal server error occurred." }
                });
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(loginDto);
                if (result == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);
                return Ok(result);
            }
            // No longer catch UnauthorizedAccessException for unverified email. Allow login for unverified users.
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user login");
                return StatusCode(500, "An error occurred during login");
            }
        }

        /// <summary>
        /// Verify email address
        /// </summary>
        /// <param name="verifyEmailDto">Email verification details</param>
        /// <returns>Success or failure response</returns>
        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.VerifyEmailAsync(verifyEmailDto.UserId, verifyEmailDto.Token);
                if (result)
                {
                    return Ok(new { message = "Email verified successfully" });
                }

                return BadRequest("Email verification failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during email verification");
                return StatusCode(500, "An error occurred during email verification");
            }
        }

        /// <summary>
        /// Request password reset
        /// </summary>
        /// <param name="forgotPasswordDto">Email for password reset</param>
        /// <returns>Success response</returns>
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
                if (result)
                {
                    return Ok(new { message = "Password reset email sent if account exists" });
                }

                return BadRequest("Failed to send password reset email");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset request");
                return StatusCode(500, "An error occurred during password reset request");
            }
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="resetPasswordDto">Password reset details</param>
        /// <returns>Success or failure response</returns>
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ResetPasswordAsync(resetPasswordDto);
                if (result)
                {
                    return Ok(new { message = "Password reset successfully" });
                }

                return BadRequest("Password reset failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset");
                return StatusCode(500, "An error occurred during password reset");
            }
        }

        /// <summary>
        /// Resend email verification
        /// </summary>
        /// <param name="resendVerificationEmailDto">Email to resend verification</param>
        /// <returns>Success or failure response</returns>
        [HttpPost("resend-verification")]
        public async Task<ActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailDto resendVerificationEmailDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ResendVerificationEmailAsync(resendVerificationEmailDto.Email);
                if (result)
                {
                    return Ok(new { message = "Verification email sent" });
                }

                return BadRequest("Failed to resend verification email");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during resend verification email");
                return StatusCode(500, "An error occurred during resend verification email");
            }
        }

        /// <summary>
        /// Clear all users from the database (Development only)
        /// </summary>
        /// <returns>Success message</returns>
        [HttpDelete("clear-all-users")]
        public async Task<ActionResult> ClearAllUsers()
        {
            try
            {
                // Only allow in development environment
                if (!_hostEnvironment.IsDevelopment())
                {
                    return Forbid("This endpoint is only available in development environment");
                }

                var users = _userManager.Users.ToList();
                foreach (var user in users)
                {
                    await _userManager.DeleteAsync(user);
                }

                _logger.LogInformation("All users cleared from database");
                return Ok(new { message = $"Successfully cleared {users.Count} users from the database" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing all users");
                return StatusCode(500, "An error occurred while clearing users");
            }
        }

        /// <summary>
        /// Get current authenticated user info
        /// </summary>
        /// <returns>Current user info</returns>
        [HttpGet("me")]
        public async Task<ActionResult<AuthResponseDto>> Me()
        {
            try
            {
                var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var roles = await _userManager.GetRolesAsync(user);

                // Extract token from Authorization header
                string token = string.Empty;
                if (Request.Headers.ContainsKey("Authorization"))
                {
                    var authHeader = Request.Headers["Authorization"].ToString();
                    if (authHeader.StartsWith("Bearer "))
                    {
                        token = authHeader.Substring("Bearer ".Length).Trim();
                    }
                }

                // Fetch refresh token and expiration from AuthService (in-memory or DB)
                string refreshToken = string.Empty;
                DateTime refreshTokenExp = DateTime.MinValue;
                if (_authService is BookStore.API.Services.AuthService concreteAuthService)
                {
                    var tokensField = typeof(BookStore.API.Services.AuthService).GetField("_refreshTokens", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    if (tokensField != null)
                    {
                        var tokens = tokensField.GetValue(null) as System.Collections.Generic.Dictionary<string, (string RefreshToken, DateTime Expiration)>;
                        if (tokens != null && tokens.TryGetValue(user.Id, out var tuple))
                        {
                            refreshToken = tuple.RefreshToken;
                            refreshTokenExp = tuple.Expiration;
                        }
                    }
                }

                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles,
                    Expiration = DateTime.UtcNow.AddHours(1), // Not used here, but required by DTO
                    Token = token,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshTokenExp
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching current user info");
                return StatusCode(500, new { message = "An error occurred while fetching user info" });
            }
        }

        private static string GetUserFriendlyMessage(string errorCode)
        {
            return errorCode switch
            {
                "USER_EXISTS" => "A user with this email address already exists. Please use a different email or try logging in.",
                "VALIDATION_ERROR" => "Registration failed due to validation errors. Please check your input and try again.",
                "INTERNAL_ERROR" => "An internal error occurred. Please try again later.",
                _ => "Registration failed. Please check your information and try again."
            };
        }
    }
}
