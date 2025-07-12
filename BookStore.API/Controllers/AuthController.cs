using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookStore.API.DTOs;
using BookStore.API.Services.Interfaces;
using BookStore.API.Models;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
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
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", registerDto?.Email);
                
                if (registerDto == null)
                {
                    _logger.LogWarning("Registration failed: registerDto is null");
                    return BadRequest("Registration data is required");
                }
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Registration failed due to model validation errors for {Email}: {Errors}", 
                        registerDto.Email, 
                        string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(registerDto);
                if (result == null)
                {
                    _logger.LogWarning("Registration service returned null for {Email}", registerDto.Email);
                    return BadRequest("Registration failed. User might already exist.");
                }

                _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration");
                return StatusCode(500, "An error occurred during registration");
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
    }
}
