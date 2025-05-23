﻿using BusinessLogic.DTOs;
using BusinessLogic.Services.Test.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Test
{
    public interface ITestService
    {
        Task<ResultDto> AddTestAsync(TestAddDto dto);
        Task<ResultDto> DeleteTestAsync(int id);
        Task<ResultDto> GetAllTestsAsync();
        Task<ResultDto> GetTestByIdAsync(int id);
        Task<ResultDto> UpdateTestAsync(int id, TestAddDto dto);

        Task<ResultDto> ViewTestAsync(int testId);
    }
}
