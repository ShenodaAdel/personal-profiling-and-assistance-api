using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BusinessLogic.Services.Implementation
{
    public class QuestionService : IQuestionService
    {
        private readonly MyDbContext _context;

        public QuestionService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<ResultDto> AddQuestionAsync(QuestionDto dto)
        {
            // Validate the input (e.g., ensure Content is not empty)
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return new ResultDto
                {
                    HasError = true,
                    ErrorMessage = "Content cannot be empty."
                };
            }

            var question = new Question
            {
                Content = dto.Content
            };

            // Add the question to the context
            await _context.Questions.AddAsync(question);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Result = question,
                    NotFound = false,
                    HasError = false
                };
            }

            return new ResultDto
            {
                NotFound = false,
                HasError = true,
                ErrorMessage = "Question could not be added."
            };
        }

        // End of AddQuestionAsync method

        public async Task<ResultDto> DeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return new ResultDto
                {
                    NotFound = true,
                    HasError = true,
                    ErrorMessage = "Question not found."
                };
            }

            _context.Questions.Remove(question);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    NotFound = false,
                    HasError = false,
                    Result = "Question deleted successfully."
                };
            }

            return new ResultDto
            {
                NotFound = false,
                HasError = true,
                ErrorMessage = "Question could not be deleted."
            };
        }

        // End of DeleteQuestionAsync method

        public async Task<ResultDto> UpdateQuestionAsync(int id, QuestionDto dto)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return new ResultDto
                {
                    NotFound = true,
                    HasError = true,
                    ErrorMessage = "Question not found."
                };
            }


            // Check if the Name already exists in the database (excluding the current test)
            bool ContentExists = await _context.Questions.AnyAsync(q => q .Content == dto.Content && q.Id != id);
            if (ContentExists)
            {
                return new ResultDto
                {
                    NotFound = false,
                    HasError = true,
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
                    Result = question,
                    NotFound = false,
                    HasError = false
                };
            }

            return new ResultDto
            {
                NotFound = false,
                HasError = true,
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
                    NotFound = true,
                    HasError = false,
                    ErrorMessage = "Question not found."
                };
            }

            return new ResultDto
            {
                Result = question,
                NotFound = false,
                HasError = false
            };
        }

        // End of GetQuestionByIdAsync method

        public async Task<ResultDto> GetAllQuestionsAsync()
        {
            var questions = await _context.Questions.ToListAsync();

            return questions.Any()
            ? new ResultDto
                {
                    Result = questions,
                    NotFound = false,
                    HasError = false
                }
                : new ResultDto
                {
                    NotFound = true,
                    HasError = false,
                    ErrorMessage = "No questions found."
                };
        }

        // End of GetAllQuestionsAsync method
    }
}
