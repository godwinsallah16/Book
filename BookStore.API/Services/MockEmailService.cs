using BookStore.API.Services.Interfaces;

namespace BookStore.API.Services
{
    public class MockEmailService : IEmailService
    {
        private readonly ILogger<MockEmailService> _logger;

        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("MOCK EMAIL - To: {Email}, Subject: {Subject}", email, subject);
            _logger.LogInformation("MOCK EMAIL - Body: {Body}", htmlMessage);
            return Task.CompletedTask;
        }

        public Task SendEmailVerificationAsync(string email, string firstName, string verificationLink)
        {
            _logger.LogInformation("MOCK EMAIL VERIFICATION - To: {Email}, Name: {FirstName}", email, firstName);
            _logger.LogInformation("MOCK EMAIL VERIFICATION - Verification Link: {Link}", verificationLink);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetAsync(string email, string firstName, string resetLink)
        {
            _logger.LogInformation("MOCK PASSWORD RESET - To: {Email}, Name: {FirstName}", email, firstName);
            _logger.LogInformation("MOCK PASSWORD RESET - Reset Link: {Link}", resetLink);
            return Task.CompletedTask;
        }

        public Task SendWelcomeEmailAsync(string email, string firstName)
        {
            _logger.LogInformation("MOCK WELCOME EMAIL - To: {Email}, Name: {FirstName}", email, firstName);
            return Task.CompletedTask;
        }
    }
}
