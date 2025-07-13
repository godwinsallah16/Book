using System.Net;
using System.Net.Mail;
using System.Text;
using BookStore.API.Services.Interfaces;

namespace BookStore.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var fromEmail = emailSettings["From"];
                var fromName = emailSettings["FromName"];
                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = int.Parse(emailSettings["SmtpPort"]!);
                var smtpUsername = emailSettings["SmtpUsername"];
                var smtpPassword = emailSettings["SmtpPassword"];
                var enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");
                var useDefaultCredentials = bool.Parse(emailSettings["UseDefaultCredentials"] ?? "false");

                using var smtpClient = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = useDefaultCredentials,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!, fromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", email);
                throw;
            }
        }

        public async Task SendEmailVerificationAsync(string email, string firstName, string verificationLink)
        {
            var subject = "Verify Your Email Address - BookStore";
            var htmlMessage = GenerateEmailVerificationHtml(firstName, verificationLink);
            await SendEmailAsync(email, subject, htmlMessage);
        }

        public async Task SendPasswordResetAsync(string email, string firstName, string resetLink)
        {
            var subject = "Reset Your Password - BookStore";
            var htmlMessage = GeneratePasswordResetHtml(firstName, resetLink);
            await SendEmailAsync(email, subject, htmlMessage);
        }

        public async Task SendWelcomeEmailAsync(string email, string firstName)
        {
            var subject = "Welcome to BookStore!";
            var htmlMessage = GenerateWelcomeEmailHtml(firstName);
            await SendEmailAsync(email, subject, htmlMessage);
        }

        private string GenerateEmailVerificationHtml(string firstName, string verificationLink)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 40px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .button {{ display: inline-block; padding: 12px 30px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                        .footer {{ margin-top: 30px; text-align: center; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>📚 BookStore</h1>
                        </div>
                        <h2>Hello {firstName}!</h2>
                        <p>Thank you for registering with BookStore. Please verify your email address by clicking the button below:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{verificationLink}' style='text-decoration: none;' target='_blank'>
                                <button style='display: inline-block; padding: 12px 30px; background-color: #007bff; color: white; border: none; border-radius: 5px; font-weight: bold; font-size: 16px; cursor: pointer;'>
                                    Verify Email Address
                                </button>
                            </a>
                        </div>
                        <p>If you didn't create an account with us, please ignore this email.</p>
                        <p>This link will expire in 24 hours for security reasons.</p>
                        <div class='footer'>
                            <p>Best regards,<br>The BookStore Team</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GeneratePasswordResetHtml(string firstName, string resetLink)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 40px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .button {{ display: inline-block; padding: 12px 30px; background-color: #dc3545; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                        .footer {{ margin-top: 30px; text-align: center; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>📚 BookStore</h1>
                        </div>
                        <h2>Hello {firstName}!</h2>
                        <p>You recently requested to reset your password. Click the button below to reset it:</p>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='{resetLink}' class='button'>Reset Password</a>
                        </p>
                        <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>
                        <p>This link will expire in 1 hour for security reasons.</p>
                        <div class='footer'>
                            <p>Best regards,<br>The BookStore Team</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GenerateWelcomeEmailHtml(string firstName)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 40px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .footer {{ margin-top: 30px; text-align: center; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>📚 BookStore</h1>
                        </div>
                        <h2>Welcome to BookStore, {firstName}!</h2>
                        <p>Thank you for verifying your email address. You can now enjoy all the features of our BookStore platform:</p>
                        <ul>
                            <li>Browse our extensive collection of books</li>
                            <li>Add books to your favorites</li>
                            <li>Purchase books securely</li>
                            <li>Track your orders</li>
                        </ul>
                        <p>Happy reading!</p>
                        <div class='footer'>
                            <p>Best regards,<br>The BookStore Team</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
