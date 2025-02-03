using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface ITestService
    {
        Task<ResultDto> AddTestAsync(TestDto dto);
        Task<ResultDto> DeleteTestAsync(int id);
        Task<ResultDto> GetAllTestsAsync();
        Task<ResultDto> GetTestByIdAsync(int id);
        Task<ResultDto> UpdateTestAsync(int id, TestDto dto);
    }
}
