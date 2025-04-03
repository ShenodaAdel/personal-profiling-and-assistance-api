using BusinessLogic.DTOs;
using BusinessLogic.Services.Question.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Question
{
    public interface IQuestionService
    {
        Task<ResultDto> AddQuestionAsync(int testId , QuestionAddDto dto);
        Task<ResultDto> DeleteQuestionAsync(int id);
        Task<ResultDto> GetAllQuestionsAsync();
        Task<ResultDto> GetQuestionByIdAsync(int id);
        Task<ResultDto> EditQuestionWithChoicesAsync(int questionId, QuestionAddWithChoicesDto dto);
        Task<ResultDto> AddQuestionWithChoicesAsync(int testId, QuestionAddWithChoicesDto dto);
    }
}
