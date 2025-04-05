using BusinessLogic.DTOs;
using BusinessLogic.Services.UserTest.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.UserTest
{
    public interface IUserTestService
    {
        Task<ResultDto> AddUserTestAsync(string userId , int testId , UserTestDto dto);

        Task<ResultDto> UpdateUserTestAsync(int id, UpdateUserTestDto dto);
        Task<ResultDto> DeleteUserTestAsync(int id);
        
        Task<ResultDto> GetUserTestByIdAsync(string id);
        Task<ResultDto> GetAllUserTestsAsync();
        
    }
}
