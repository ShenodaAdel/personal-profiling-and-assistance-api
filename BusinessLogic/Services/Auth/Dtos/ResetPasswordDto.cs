namespace BusinessLogic.Services.Auth.Dtos
{
    public class ResetPasswordDto
    {
        public string? Email { get; set; }
        public string? OtpCode { get; set; }
        public string? NewPassword { get; set; }
    }
}
