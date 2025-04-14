using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
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
            var email = new MimeMessage 
            { 
                Sender = MailboxAddress.Parse(_emailSettings.Email), // Sender can send email 
                Subject = "Reset Password",
            };
            email.To.Add(MailboxAddress.Parse(toEmail)); // recived email 

            var builder = new BodyBuilder();

            if (!string.IsNullOrEmpty(resetLink))
            {
                // The reset link to be used by the user to reset their password
                builder.HtmlBody = $"<p>Please click the link below to reset your password:</p><a href='{resetLink}'>Reset Password</a>";
            }

            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email)); // Email header 

            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Email, _emailSettings.Password); // Authentication
            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }

    }
}
