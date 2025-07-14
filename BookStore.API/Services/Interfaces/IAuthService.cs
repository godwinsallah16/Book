using BookStore.API.DTOs;

namespace BookStore.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<string> GenerateJwtTokenAsync(string email, string userId);
        Task<string> GenerateRefreshTokenAsync();
        Task<AuthResponseDto?> RefreshTokenAsync(string userId, string refreshToken);
        Task<bool> VerifyEmailAsync(string userId, string token);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> ResendVerificationEmailAsync(string email);
    }
}
