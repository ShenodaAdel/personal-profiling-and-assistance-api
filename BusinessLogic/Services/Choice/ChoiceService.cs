using BusinessLogic.DTOs;
using Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Choice
{
    public class ChoiceService : IChoiceService
    {
        private readonly MyDbContext _context;

        public ChoiceService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<ResultDto> AddChoiceAsync(ChoiceAddDto dto)
        {
            // Validate the input (e.g., ensure Content is not empty)
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Content cannot be empty."
                };
            }
            var choice = new Data.Models.Choice
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
                    Data = new
                    {
                        Content = choice.Content,
                        ChoiceId = choice.Id
                    },
                    Success = true
                };
            }
            return new ResultDto
            {
                Success = false,
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
                    Success = false,
                    ErrorMessage = "Choice not found."
                };
            }
            _context.Choices.Remove(choice);
            int saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Data = new
                    {
                        Content = choice.Content,
                        ChoiceId = choice.Id
                    },
                    Success = true
                };
            }
            return new ResultDto
            {
                Success = false,
                ErrorMessage = "Choice could not be deleted."
            };
        }
        // End of DeleteChoiceAsync method
        public async Task<ResultDto> UpdateChoiceAsync(int id, ChoiceAddDto dto)
        {
            var choice = await _context.Choices.FindAsync(id);
            if (choice == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Choice not found."
                };
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return new ResultDto
                {
                    Success = false,
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
                    Data = new {
                        NewContent = choice.Content,
                        Id = choice.Id
                    },
                    Success = true
                };
            }

            return new ResultDto
            {
                Success = false,
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
                    Success = false,
                    ErrorMessage = "Choice not found."
                };
            }
            return new ResultDto
            {
                Data = new
                {
                    Content = choice.Content,
                    ChoiceId = choice.Id
                },
                Success = true
            };
        }

        // End of GetChoiceByIdAsync method

        public async Task<ResultDto> GetAllChoicesAsync()
        {
            var choices = await _context.Choices
                .Select(c => new { c.Id, c.Content })
                .ToListAsync();

            return new ResultDto
            {
                Data = choices,
                Success = true
            };
        }


        // End of GetAllChoicesAsync method
    }
}