namespace BusinessLogic.Services.Emails
{
    public interface IEmailServices
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
