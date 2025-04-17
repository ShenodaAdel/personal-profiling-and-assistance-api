using BusinessLogic.DTOs;

namespace BusinessLogic.Services.OTP_Service
{
    public interface IOTPService
    {
        Task<ResultDto> SendOtpToEmailAsync(string email);
        Task<ResultDto> VerifyOtpAndResetPasswordAsync(string email, string code, string newPassword);
    }
}
