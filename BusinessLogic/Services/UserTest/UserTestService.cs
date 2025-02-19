using BusinessLogic.DTOs;
using BusinessLogic.Services.UserTest.Dtos;
using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.UserTest
{
    public class UserTestService : IUserTestService
    {
        private readonly MyDbContext _context;

        public UserTestService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<ResultDto> AddUserTestAsync(UserTestDto dto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Result))
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "Result cannot be empty."
                };
            }

            // Validate UserId
            if (dto.UserId == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "UserId must be provided."
                };
            }

            // Validate TestId
            if (dto.TestId == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "TestId must be provided."
                };
            }

            // Check if User exists
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "User not found."
                };
            }

            // Check if Test exists
            var test = await _context.Tests.FindAsync(dto.TestId);
            if (test == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "Test not found."
                };
            }

            // Create the new UserTest entity
            var userTest = new Data.Models.UserTest
            {
                Date = dto.Date,
                Result = dto.Result,
                UserId = dto.UserId,
                TestId = dto.TestId
            };

            await _context.UserTests.AddAsync(userTest);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Success = true,
                    Data = userTest
                };
            }

            return new ResultDto
            {
                Success = false,
                ErrorMessage = "UserTest could not be added."
            };
        }

        // End of AddUserTestAsync method

        public async Task<ResultDto> UpdateUserTestAsync(int id, UserTestDto dto)
        {
            // Find the existing UserTest
            var userTest = await _context.UserTests.FindAsync(id);
            if (userTest == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "UserTest not found."
                };
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Result))
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "Result cannot be empty."
                };
            }

            // Optionally, validate that UserId and TestId still refer to valid entities
            if (dto.UserId.HasValue)
            {
                var user = await _context.Users.FindAsync(dto.UserId.Value);
                if (user == null)
                {
                    return new ResultDto
                    {
                        Success = true,
                        ErrorMessage = "User not found."
                    };
                }
            }

            if (dto.TestId.HasValue)
            {
                var test = await _context.Tests.FindAsync(dto.TestId.Value);
                if (test == null)
                {
                    return new ResultDto
                    {
                        Success = true,
                        ErrorMessage = "Test not found."
                    };
                }
            }

            // Update fields
            userTest.Date = dto.Date;
            userTest.Result = dto.Result;
            userTest.UserId = dto.UserId;
            userTest.TestId = dto.TestId;

            _context.UserTests.Update(userTest);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Success = true,
                    Data = userTest
                };
            }

            return new ResultDto
            {
                Success = true,
                ErrorMessage = "UserTest could not be updated."
            };
        }

        // End of UpdateUserTestAsync method

        public async Task<ResultDto> DeleteUserTestAsync(int id)
        {
            // Find the existing UserTest
            var userTest = await _context.UserTests.FindAsync(id);
            if (userTest == null)
            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "UserTest not found."
                };
            }

            try
            {
                _context.UserTests.Remove(userTest);
                int saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    return new ResultDto
                    {
                        Success = true,
                        ErrorMessage = "UserTest deleted successfully.",
                        Data = userTest
                    };
                }

                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "UserTest could not be deleted."
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


        // End of DeleteUserTestAsync method

        public async Task<ResultDto> GetUserTestByIdAsync(int id)
        {
            var userTest = await _context.UserTests
                .Include(ut => ut.User)
                .Include(ut => ut.Test)
                .FirstOrDefaultAsync(ut => ut.Id == id);

            if (userTest == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "UserTest not found."
                };
            }

            return new ResultDto
            {
                Success = true,
                Data = userTest
            };
        }

        // End of GetUserTestByIdAsync

        public async Task<ResultDto> GetAllUserTestsAsync()
        {
            var userTests = await _context.UserTests
                .Include(ut => ut.User)
                .Include(ut => ut.Test)
                .ToListAsync();

            if (!userTests.Any())
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "No UserTests found."
                };
            }

            return new ResultDto
            {
                Success = true,
                Data = userTests
            };
        }

        // End of GetAllUserTestsAsync
    }
}
