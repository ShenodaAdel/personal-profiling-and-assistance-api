using BusinessLogic.DTOs;
using BusinessLogic.Services.User.Dtos;
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
        Task<ResultDto> UpdateUserAsync(string id, ApplicationUser dto);  

    }
}
