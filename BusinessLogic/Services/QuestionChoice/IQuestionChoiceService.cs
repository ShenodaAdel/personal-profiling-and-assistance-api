using BusinessLogic.DTOs;
using BusinessLogic.Services.QuestionChoice.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.QuestionChoice
{
    public interface IQuestionChoiceService
    {
        Task<ResultDto> AddQuestionChoiceAsync(QuestionChoiceDto dto);
        Task<ResultDto> UpdateQuestionChoiceAsync(int id, QuestionChoiceDto dto);
        Task<ResultDto> DeleteQuestionChoiceAsync(int id);
        Task<ResultDto> GetQuestionChoiceByIdAsync(int id);
        Task<ResultDto> GetAllQuestionChoicesAsync();
    }
}
