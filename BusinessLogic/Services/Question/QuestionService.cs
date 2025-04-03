﻿using BusinessLogic.DTOs;
using BusinessLogic.Services.Question.Dtos;
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

        public async Task<ResultDto> AddQuestionAsync(int testId, QuestionAddDto dto)
        {

            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Content cannot be empty."
                };
            }

            // Check if the Test exists
            var testExists = await _context.Tests.AnyAsync(t => t.Id == testId);
            if (!testExists)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test not found."
                };
            }

            if (testId <= 0)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "TestId Must be Positive number."
                };
            }




            var question = new Data.Models.Question
            {
                Content = dto.Content,
                TestId = testId
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
            var question = await _context.Questions
                .Include(q => q.QuestionChoices)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Question not found."
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get the related QuestionChoices (exactly 4 per question)
                var questionChoices = question.QuestionChoices.ToList();

                // Remove QuestionChoices (unlinking choices from this question)
                _context.QuestionChoices.RemoveRange(questionChoices);

                // Remove the Question itself
                _context.Questions.Remove(question);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResultDto
                {
                    Success = true,
                    Data = new
                    {
                        QuestionId = id,
                        Message = "Question and its related question choices deleted successfully."
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "An error occurred while deleting the question and its choices: " + ex.Message
                };
            }
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
            var question = await _context.Questions
                .Include(q => q.QuestionChoices)
                .ThenInclude(qc => qc.Choice)
                .FirstOrDefaultAsync(q => q.Id == id);

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
                    question.TestId,
                    Choices = question.QuestionChoices.Select(qc => new
                    {
                        qc.Choice.Id,
                        qc.Choice.Content
                    }).ToList()
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
                    TestId = q.TestId,
                    TestName = q.Test.Name
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

        public async Task<ResultDto> AddQuestionWithChoicesAsync(int testId, QuestionAddWithChoicesDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content)) // Check if the Content for question is empty
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Question content cannot be empty."
                };
            }

            if (testId <= 0)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "TestId must be a positive number."
                };
            }

            // Check if the Test exists
            var testExists = await _context.Tests.AnyAsync(t => t.Id == testId);
            if (!testExists)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test not found."
                };
            }

            if (dto.Choices == null || dto.Choices.Count != 4)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Exactly four choices must be provided."
                };
            }

            // Check if the question already exists
            var existingQuestion = await _context.Questions
                .FirstOrDefaultAsync(q => q.TestId == testId && q.Content == dto.Content);

            if (existingQuestion != null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Question already exists."
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create the Question
                var question = new Data.Models.Question
                {
                    Content = dto.Content,
                    TestId = testId
                };

                await _context.Questions.AddAsync(question);
                await _context.SaveChangesAsync();

                // Create Choices and link them to the Question
                var questionChoices = new List<Data.Models.QuestionChoice>();
                var existingChoices = new List<Data.Models.Choice>();

                foreach (var choiceContent in dto.Choices)
                {
                    // Check if the choice already exists
                    var existingChoice = await _context.Choices
                        .FirstOrDefaultAsync(c => c.Content == choiceContent);

                    Data.Models.Choice choice;
                    if (existingChoice != null)
                    {
                        // If the choice already exists, use its ID
                        choice = existingChoice;
                    }
                    else
                    {
                        // If the choice doesn't exist, create a new one
                        choice = new Data.Models.Choice { Content = choiceContent };
                        await _context.Choices.AddAsync(choice);
                        await _context.SaveChangesAsync(); // Ensure ID is generated
                    }

                    // Link choice to the question
                    var questionChoice = new Data.Models.QuestionChoice
                    {
                        QuestionId = question.Id,
                        ChoiceId = choice.Id
                    };
                    questionChoices.Add(questionChoice);
                }

                await _context.QuestionChoices.AddRangeAsync(questionChoices);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ResultDto
                {
                    Data = new
                    {
                        QuestionId = question.Id,
                        Content = question.Content,
                        TestId = question.TestId,
                        Choices = dto.Choices.Select((c, index) => new { ChoiceNumber = index + 1, Content = c }).ToList()
                    },
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "An error occurred while adding the question and choices: " + ex.Message
                };
            }
        }

    }
}
