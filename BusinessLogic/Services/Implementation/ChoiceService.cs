using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Implementation
{
    public class ChoiceService : IChoiceService
    {
        private readonly MyDbContext _context;

        public ChoiceService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<ResultDto> AddChoiceAsync(ChoiceDto dto)
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
            var choice = new Choice
            {
                Content = dto.Content
            };
            // Add the choice to the context
            await _context.Choices.AddAsync(choice);
            int saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Result = choice,
                    NotFound = false,
                    HasError = false
                };
            }
            return new ResultDto
            {
                NotFound = false,
                HasError = true,
                ErrorMessage = "Choice could not be added."
            };
        }
        //  End of AddChoiceAsync method

        public async Task<ResultDto> DeleteChoiceAsync(int id)
        {
            var choice = await _context.Choices.FindAsync(id);
            if (choice == null)
            {
                return new ResultDto
                {
                    NotFound = true,
                    HasError = true,
                    ErrorMessage = "Choice not found."
                };
            }
            _context.Choices.Remove(choice);
            int saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Result = choice,
                    NotFound = false,
                    HasError = false
                };
            }
            return new ResultDto
            {
                NotFound = false,
                HasError = true,
                ErrorMessage = "Choice could not be deleted."
            };
        }
        // End of DeleteChoiceAsync method
        public async Task<ResultDto> UpdateChoiceAsync(int id, ChoiceDto dto)
        {
            var choice = await _context.Choices.FindAsync(id);
            if (choice == null)
            {
                return new ResultDto
                {
                    NotFound = true,
                    HasError = true,
                    ErrorMessage = "Choice not found."
                };
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return new ResultDto
                {
                    HasError = true,
                    ErrorMessage = "Content cannot be empty."
                };
            }

            // Update the content
            choice.Content = dto.Content;
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Result = choice,
                    NotFound = false,
                    HasError = false
                };
            }

            return new ResultDto
            {
                NotFound = false,
                HasError = true,
                ErrorMessage = "Choice could not be updated."
            };
        }

        // End of UpdateChoiceAsync method

        public async Task<ResultDto> GetChoiceByIdAsync(int id)
        {
            var choice = await _context.Choices.FindAsync(id);
            if (choice == null)
            {
                return new ResultDto
                {
                    NotFound = true,
                    HasError = true,
                    ErrorMessage = "Choice not found."
                };
            }
            return new ResultDto
            {
                Result = choice,
                NotFound = false,
                HasError = false
            };
        }

        // End of GetChoiceByIdAsync method

        public async Task<ResultDto> GetAllChoicesAsync()
        {
            var choices = _context.Choices.ToList();
            return new ResultDto
            {
                Result = choices,
                NotFound = false,
                HasError = false
            };
        }

        // End of GetAllChoicesAsync method
    }


}
