﻿using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<ResultDto> AddQuestionAsync(QuestionDto dto);
        Task<ResultDto> DeleteQuestionAsync(int id);
        Task<ResultDto> GetAllQuestionsAsync();
        Task<ResultDto> GetQuestionByIdAsync(int id);
        Task<ResultDto> UpdateQuestionAsync(int id, QuestionDto dto);
    }
}
