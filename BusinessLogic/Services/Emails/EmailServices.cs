using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using BusinessLogic.Services.Emails.Dtos;
using BusinessLogic.Services.Auth;


namespace BusinessLogic.Services.Emails
{
    public class EmailService : IEmailServices
    {
        private readonly EmailSettingsDto _emailSettings;
        private readonly ILogger<AuthService> _logger;

        public EmailService(IOptions<EmailSettingsDto> emailSettings, ILogger<AuthService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink)
        {
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer) ||
                !_emailSettings.SmtpPort.HasValue ||
                string.IsNullOrEmpty(_emailSettings.SmtpUser) ||
                string.IsNullOrEmpty(_emailSettings.SmtpPass))
            {
                throw new InvalidOperationException("Email settings are not configured properly.");
            }

            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort.Value))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass);

                var mail = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail ?? _emailSettings.SmtpUser, _emailSettings.FromName ?? "Support"),
                    Subject = "Reset your password",
                    Body = $"Please reset your password by clicking this link: {resetLink}",
                    IsBodyHtml = false
                };
                mail.To.Add(toEmail);

                try
                {
                    _logger.LogInformation($"Attempting to send reset password email to {toEmail}.");
                    await client.SendMailAsync(mail);
                    _logger.LogInformation("Password reset email sent successfully.");
                }
                catch (SmtpException smtpEx)
                {
                    // Log detailed SMTP exception
                    _logger.LogError(smtpEx, "SMTP error occurred while sending password reset email.");
                    throw new InvalidOperationException("Failed to send password reset email due to SMTP error.", smtpEx);
                }
                catch (Exception ex)
                {
                    // Log general exceptions
                    _logger.LogError(ex, "General error occurred while sending password reset email.");
                    throw new InvalidOperationException("Failed to send password reset email due to a general error.", ex);
                }
            }
        }

    }
}
