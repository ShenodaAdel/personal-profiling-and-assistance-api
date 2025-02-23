using BusinessLogic.DTOs;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BusinessLogic.Services.Question
{
    public class QuestionService : IQuestionService
    {
        private readonly MyDbContext _context; 

        public QuestionService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<ResultDto> AddQuestionAsync(QuestionAddDto dto)
        {
            
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Content cannot be empty."
                };
            }

            
            if (!dto.TestId.HasValue)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "TestId is required."
                };
            }

            var testExists = await _context.Tests.AnyAsync(t => t.Id == dto.TestId.Value);
            if (!testExists)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test not found."
                };
            }

            
            var question = new Data.Models.Question
            {
                Content = dto.Content,
                TestId = dto.TestId.Value 
            };


            await _context.Questions.AddAsync(question);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Data = new
                    {
                        QuestionId = question.Id,
                        Content = question.Content,
                        TestId = question.TestId
                    },
                    Success = true
                };
            }

            return new ResultDto
            {
                Success = false,
                ErrorMessage = "Question could not be added."
            };
        }

        // End of AddQuestionAsync method

        // must ada new check if the new question is already in the database or not

        public async Task<ResultDto> DeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Question not found."
                };
            }

            _context.Questions.Remove(question);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Success = true,
                    Data = new
                    {
                        question.Id,
                        question.Content
                    }
                };
            }

            return new ResultDto
            {  
                Success = false,
                ErrorMessage = "Question could not be deleted."
            };
        }

        // End of DeleteQuestionAsync method

        public async Task<ResultDto> UpdateQuestionAsync(int id, QuestionAddDto dto)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return new ResultDto
                {             
                    Success = false,
                    ErrorMessage = "Question not found."
                };
            }


            // Check if the Name already exists in the database (excluding the current test)
            bool ContentExists = await _context.Questions.AnyAsync(q => q.Content == dto.Content && q.Id != id);
            if (ContentExists)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Question Content already exists in the database."
                };
            }

            // Update the content of the question
            question.Content = dto.Content;

            _context.Questions.Update(question);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Data = new
                    {
                        question.Id,
                        question.Content,
                        question.TestId
                    },
                    Success = true
                };
            }

            return new ResultDto
            {
                Success = false,
                ErrorMessage = "Question could not be updated."
            };
        }

        // End of UpdateQuestionAsync method

        public async Task<ResultDto> GetQuestionByIdAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return new ResultDto
                {
                    
                    Success = false,
                    ErrorMessage = "Question not found."
                };
            }

            return new ResultDto
            {
                Data = new
                {
                    question.Content,
                    question.TestId
                },
                Success = true
            };
        }

        // End of GetQuestionByIdAsync method

        public async Task<ResultDto> GetAllQuestionsAsync()
        {
            var questions = await _context.Questions
                .Select(q => new
                {
                    QuestionId = q.Id,
                    Content = q.Content,
                    TestId = q.TestId
                })
                .ToListAsync();

            return questions.Any()
                ? new ResultDto
                {
                    Data = questions,
                    Success = true
                }
                : new ResultDto
                {
                    Success = false,
                    ErrorMessage = "No questions found."
                };
        }


        // End of GetAllQuestionsAsync method
    }
}
