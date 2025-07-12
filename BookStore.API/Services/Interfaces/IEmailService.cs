namespace BookStore.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendEmailVerificationAsync(string email, string firstName, string verificationLink);
        Task SendPasswordResetAsync(string email, string firstName, string resetLink);
        Task SendWelcomeEmailAsync(string email, string firstName);
    }
}
