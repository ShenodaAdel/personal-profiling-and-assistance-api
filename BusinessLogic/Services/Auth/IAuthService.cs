using BusinessLogic.DTOs;
using BusinessLogic.Services.Auth.Dtos;
using BusinessLogic.Services.User.Dtos;

namespace BusinessLogic.Services.Auth
{
    public interface IAuthService
    {
        Task<ResultDto> RegisterAsync(RegisterDto dto);
        Task<ResultDto> LoginAsync(LoginDto dto);
        Task<ResultDto> GenerateTokenAsync(string email, string password);
        Task<ResultDto> ForgotPasswordAsync(ForgetPasswordDto dto);

        Task<ResultDto> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ResultDto> LoginAdminAsync(LoginDto dto);


    }
}
