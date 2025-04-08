namespace BusinessLogic.Services.Emails
{
    public interface IEmailServices
    {
        Task SendResetPasswordEmailAsync(string toEmail, string resetLink);
    }
}
