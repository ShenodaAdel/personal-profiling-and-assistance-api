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
        Task<ResultDto> DeleteUserByTokenAsync(string Token);
        Task<ResultDto> DeleteUserByIdAsync(string userId);
        Task<ResultDto> GetAllUserAsync();
        Task<ResultDto> GetByIdUserAsync(string Token); 
        Task<ResultDto> GetByIdUserDetailsAsync(string id);
        Task<ResultDto> UpdateUserAsync(string Token, string? userName, string? phoneNumber, string? gender, byte[]? profilePicture);

        Task<ResultDto> GetAnaiysisAsync();

    }
}
