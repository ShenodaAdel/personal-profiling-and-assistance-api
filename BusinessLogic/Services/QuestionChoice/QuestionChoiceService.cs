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
                    Success = false,
                    ErrorMessage = "QuestionId must be provided."
                };
            }

            if (dto.ChoiceId == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "ChoiceId must be provided."
                };
            }

            // Check if the Question exists
            var question = await _context.Questions
                .Where(q => q.Id == dto.QuestionId)
                .Select(q => new { q.Id, q.Content })
                .FirstOrDefaultAsync();

            if (question == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Question not found."
                };
            }

            // Check if the Choice exists
            var choice = await _context.Choices
                .Where(c => c.Id == dto.ChoiceId)
                .Select(c => new { c.Id, c.Content })
                .FirstOrDefaultAsync();

            if (choice == null)
            {
                return new ResultDto
                {
                    Success = false,
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
                    Data = new
                    {
                        QuestionId = question.Id,
                        QuestionContent = question.Content,
                        ChoiceId = choice.Id,
                        ChoiceContent = choice.Content
                    }
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
            var existingQuestionChoice = await _context.QuestionChoices
                .Include(qc => qc.Question)
                .Include(qc => qc.Choice)
                .FirstOrDefaultAsync(qc => qc.Id == id);

            if (existingQuestionChoice == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "QuestionChoice not found."
                };
            }

            // Validate new QuestionId if provided
            if (dto.QuestionId.HasValue)
            {
                var question = await _context.Questions.FindAsync(dto.QuestionId.Value);
                if (question == null)
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "Question not found."
                    };
                }
                existingQuestionChoice.Question = question;  // Update question reference
                existingQuestionChoice.QuestionId = question.Id;
            }

            // Validate new ChoiceId if provided
            if (dto.ChoiceId.HasValue)
            {
                var choice = await _context.Choices.FindAsync(dto.ChoiceId.Value);
                if (choice == null)
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "Choice not found."
                    };
                }
                existingQuestionChoice.Choice = choice;  // Update choice reference
                existingQuestionChoice.ChoiceId = choice.Id;
            }

            _context.QuestionChoices.Update(existingQuestionChoice);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Success = true,
                    Data = new
                    {
                        QuestionId = existingQuestionChoice.Question.Id,
                        QuestionContent = existingQuestionChoice.Question.Content,
                        ChoiceId = existingQuestionChoice.Choice.Id,
                        ChoiceContent = existingQuestionChoice.Choice.Content
                    }
                };
            }

            return new ResultDto
            {
                Success = false,
                ErrorMessage = "QuestionChoice could not be updated."
            };
        }


        // End of UpdateQuestionChoiceAsync

        public async Task<ResultDto> DeleteQuestionChoiceAsync(int id)
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

            try
            {
                // Capture the deleted data
                var deletedData = new
                {
                    QuestionId = questionChoice.Question.Id,
                    QuestionContent = questionChoice.Question.Content,
                    ChoiceId = questionChoice.Choice.Id,
                    ChoiceContent = questionChoice.Choice.Content
                };

                _context.QuestionChoices.Remove(questionChoice);
                int saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    return new ResultDto
                    {
                        Success = true,
                        Data = deletedData
                    };
                }

                return new ResultDto
                {
                    Success = false,
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
                .Where(qc => qc.Id == id)
                .Select(qc => new
                {
                    QuestionId = qc.Question.Id,
                    QuestionContent = qc.Question.Content,
                    ChoiceId = qc.Choice.Id,
                    ChoiceContent = qc.Choice.Content
                })
                .FirstOrDefaultAsync();

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
                .ThenInclude(q => q.Test) // Ensure Test is included
                .Include(qc => qc.Choice)
                .Select(qc => new
                {
                    Id = qc.Id,  // ID of the QuestionChoice
                    QuestionId = qc.Question.Id,
                    QuestionContent = qc.Question.Content,
                    ChoiceId = qc.Choice.Id,
                    ChoiceContent = qc.Choice.Content,
                    TestId = qc.Question.Test.Id, // Assuming Test has an Id field
                    TestName = qc.Question.Test.Name // Assuming Test has a Name field
                })
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
