using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs
{
    public class RefreshTokenRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
            ErrorMessage = "Password must contain at least 8 characters including uppercase, lowercase, digit and special character.")]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public bool EmailConfirmed { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class AuthResultDto
    {
        public bool Success { get; set; }
        public AuthResponseDto? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public string ErrorCode { get; set; } = string.Empty;
    }

    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
            ErrorMessage = "Password must contain at least 8 characters including uppercase, lowercase, digit and special character.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class VerifyEmailDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }

    public class ResendVerificationEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
