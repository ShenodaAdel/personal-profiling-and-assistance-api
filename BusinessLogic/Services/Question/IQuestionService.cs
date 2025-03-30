using BusinessLogic.DTOs;
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
        Task<ResultDto> UpdateQuestionAsync(int id, QuestionAddDto dto);
    }
}
