using BusinessLogic.DTOs;
using BusinessLogic.Services.User.Dtos;
using BusinessLogic.Services.User.DTOs;
using Data.Models;
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
        Task<ResultDto> DeleteUserByIdAsync(string id);
        Task<ResultDto> GetAllUserAsync();
        Task<ResultDto> GetByIdUserAsync(string id);
        Task<ResultDto> GetByIdUserDetailsAsync(string id);
        Task<ResultDto> UpdateUserAsync(string id, string? userName, string? email, string? phoneNumber, string? gender, byte[]? profilePicture);  

    }
}
