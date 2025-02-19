using BusinessLogic.DTOs;
using BusinessLogic.Services.QuestionChoice.Dtos;
using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.QuestionChoice
{
    public class QuestionChoiceService : IQuestionChoiceService
    {
        private readonly MyDbContext _context;

        public QuestionChoiceService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<ResultDto> AddQuestionChoiceAsync(QuestionChoiceDto dto)
        {
            // Validate required fields
            if (dto.QuestionId == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "QuestionId must be provided."
                };
            }

            if (dto.ChoiceId == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "ChoiceId must be provided."
                };
            }

            // Check if the Question exists
            var question = await _context.Questions.FindAsync(dto.QuestionId.Value);
            if (question == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "Question not found."
                };
            }

            // Check if the Choice exists
            var choice = await _context.Choices.FindAsync(dto.ChoiceId.Value);
            if (choice == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "Choice not found."
                };
            }

            var questionChoice = new Data.Models.QuestionChoice
            {
                QuestionId = dto.QuestionId,
                ChoiceId = dto.ChoiceId
            };

            await _context.QuestionChoices.AddAsync(questionChoice);
            int saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Success = true,
                    Data = questionChoice
                };
            }

            return new ResultDto
            {
                Success = false,
                ErrorMessage = "QuestionChoice could not be added."
            };
        }

        // End of AddQuestionChoiceAsync

        public async Task<ResultDto> UpdateQuestionChoiceAsync(int id, QuestionChoiceDto dto)
        {
            var existingQuestionChoice = await _context.QuestionChoices.FindAsync(id);
            if (existingQuestionChoice == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "QuestionChoice not found."
                };
            }

            // If provided, validate the new QuestionId
            if (dto.QuestionId.HasValue)
            {
                var question = await _context.Questions.FindAsync(dto.QuestionId.Value);
                if (question == null)
                {
                    return new ResultDto
                    {
                        Success = true,
                        ErrorMessage = "Question not found."
                    };
                }
            }

            // If provided, validate the new ChoiceId
            if (dto.ChoiceId.HasValue)
            {
                var choice = await _context.Choices.FindAsync(dto.ChoiceId.Value);
                if (choice == null)
                {
                    return new ResultDto
                    {
                        Success = true,
                        ErrorMessage = "Choice not found."
                    };
                }
            }

            // Update the fields
            existingQuestionChoice.QuestionId = dto.QuestionId ?? existingQuestionChoice.QuestionId;
            existingQuestionChoice.ChoiceId = dto.ChoiceId ?? existingQuestionChoice.ChoiceId;

            _context.QuestionChoices.Update(existingQuestionChoice);
            int saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Success = true,
                    Data = existingQuestionChoice
                };
            }

            return new ResultDto
            {
                Success = true,
                ErrorMessage = "QuestionChoice could not be updated."
            };
        }

        // End of UpdateQuestionChoiceAsync

        public async Task<ResultDto> DeleteQuestionChoiceAsync(int id)
        {
            var questionChoice = await _context.QuestionChoices.FindAsync(id);
            if (questionChoice == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "QuestionChoice not found."
                };
            }

            try
            {
                _context.QuestionChoices.Remove(questionChoice);
                int saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    return new ResultDto
                    {
                        Success = true,
                        Data = "QuestionChoice deleted successfully."
                    };
                }

                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "QuestionChoice could not be deleted."
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // End of DeleteQuestionChoiceAsync

        public async Task<ResultDto> GetQuestionChoiceByIdAsync(int id)
        {
            var questionChoice = await _context.QuestionChoices
                .Include(qc => qc.Question)
                .Include(qc => qc.Choice)
                .FirstOrDefaultAsync(qc => qc.Id == id);

            if (questionChoice == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "QuestionChoice not found."
                };
            }

            return new ResultDto
            {
                Success = true,
                Data = questionChoice
            };
        }

        // End of GetQuestionChoiceByIdAsync

        public async Task<ResultDto> GetAllQuestionChoicesAsync()
        {
            var questionChoices = await _context.QuestionChoices
                .Include(qc => qc.Question)
                .Include(qc => qc.Choice)
                .ToListAsync();

            if (!questionChoices.Any())
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "No QuestionChoices found."
                };
            }

            return new ResultDto
            {
                Success = true,
                Data = questionChoices
            };
        }

        // End of GetAllQuestionChoicesAsync

    }
}
