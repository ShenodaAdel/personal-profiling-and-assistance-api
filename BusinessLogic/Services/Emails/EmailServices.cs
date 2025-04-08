using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using BusinessLogic.Services.Emails.Dtos;

namespace BusinessLogic.Services.Emails
{
    public class EmailService : IEmailServices
    {
        private readonly EmailSettingsDto _emailSettings;

        public EmailService(IOptions<EmailSettingsDto> emailSettings)
        {
            _emailSettings = emailSettings.Value;
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

                await client.SendMailAsync(mail);
            }
        }
    }
}
