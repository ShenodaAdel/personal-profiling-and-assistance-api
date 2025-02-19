using BusinessLogic.DTOs;
using BusinessLogic.Services.User.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.User
{
    public interface IUserService
    {
        Task<ResultDto> AddUserAsync(UserAddDto dto);
        Task<ResultDto> DeleteUserByIdAsync(int id);
        Task<ResultDto> GetAllUserAsync();
        Task<ResultDto> GetByIdUserAsync(int id);
        Task<ResultDto> UpdateUserAsync(int id, Data.Models.User dto);

        Task<ResultDto> RegisterAsync(UserRegisterDto dto);

        Task<ResultDto> LoginAsync(UserLoginDto dto);
    }
}
