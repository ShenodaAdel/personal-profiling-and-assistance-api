using Microsoft.Extensions.Options;
using BusinessLogic.Services.Emails.Dtos;
using System.Net.Mail;
using System.Net;

namespace BusinessLogic.Services.Emails
{
    public class EmailService : IEmailServices
    {
        private readonly EmailSettingsDto _emailSettings;

        public EmailService(IOptions<EmailSettingsDto> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_emailSettings.Host)
                {
                    Port = _emailSettings.Port,
                    Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Email),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error while sending email: {ex.Message}");
                throw; 
            }
        }


    }
}
