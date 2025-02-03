using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResultDto> AddUserAsync(UserDto dto);
        Task<ResultDto> DeleteUserByIdAsync(int id);
        Task<ResultDto> GetAllUserAsync();
        Task<ResultDto> GetByIdUserAsync(int id);
        Task<ResultDto> UpdateUserAsync(int id ,UserDto dto);
    }
}
