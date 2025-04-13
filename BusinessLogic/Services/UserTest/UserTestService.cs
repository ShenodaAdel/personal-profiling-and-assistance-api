using BusinessLogic.DTOs;
using BusinessLogic.Services.TokenService;
using BusinessLogic.Services.UserTest.Dtos;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
        private readonly ITokenService _tokenService;

        public UserTestService(MyDbContext context,ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<ResultDto> AddUserTestAsync(string Token , int testId , UserTestDto dto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Result))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Result cannot be empty."
                };
            }

            var userId = _tokenService.GetUserIdFromToken(Token);

            // Validate UserId
            if (userId == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "UserId must be provided."
                };
            }

            // Validate TestId
            if (testId == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "TestId must be provided."
                };
            }

            // Check if User exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };
            }

            // Check if Test exists
            var test = await _context.Tests.FindAsync(testId);
            if (test == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test not found."
                };
            }

            // Create the new UserTest entity
            var userTest = new Data.Models.UserTest
            {
                Date = dto.Date,
                Result = dto.Result,
                UserId = userId,
                TestId = testId
            };

            await _context.UserTests.AddAsync(userTest);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Success = true,
                    Data = new
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Result = userTest.Result,
                        Date = userTest.Date
                    }
                };
            }

            return new ResultDto
            {
                Success = false,
                ErrorMessage = "UserTest could not be added."
            };
        }

        // End of AddUserTestAsync method

        public async Task<ResultDto> UpdateUserTestAsync(int id, UpdateUserTestDto dto)
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

            
            if (string.IsNullOrWhiteSpace(dto.Result))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Result cannot be empty."
                };
            }

            userTest.Result = dto.Result;

            _context.UserTests.Update(userTest);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Success = true,
                    Data = new
                    {
                        UserName = userTest.User?.UserName,
                        UserId = userTest.UserId,
                        TestId = userTest.TestId,
                        Result = userTest.Result,
                        Date = userTest.Date
                    }
                };
            }

            return new ResultDto
            {
                Success = false,
                ErrorMessage = "UserTest could not be updated."
            };
        }



        // End of UpdateUserTestAsync method

        public async Task<ResultDto> DeleteUserTestAsync(int id)
        {
            // Find the existing UserTest with User and Test included
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
                        Data = new
                        {
                            UserTestId = userTest.Id,
                            UserName = userTest.User?.UserName,  
                            TestName = userTest.Test.Name   
                        }
                    };
                }

                return new ResultDto
                {
                    Success = false,
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

        public async Task<ResultDto> GetUserTestByIdAsync(string userId)
        {
            var userTests = await _context.UserTests
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.Test)
                .Select(ut => new
                {
                    TestName =  ut.Test.Name,
                    Result = ut.Result,       
                    Date = ut.Date,
                    UserTestId = ut.Id 
                })
                .ToListAsync();

            if (userTests == null || userTests.Count == 0)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "No tests found for this user."
                };
            }

            return new ResultDto
            {
                Success = true,
                Data = userTests // Return the list of tests
            };
        }


        // End of GetUserTestByIdAsync

        public async Task<ResultDto> GetAllUserTestsAsync()
        {
            var userTestCounts = await _context.UserTests
                .Include(ut => ut.User)
                .GroupBy(ut => new { ut.UserId, ut.User.UserName, ut.User.Email }) // Group by User
                .Select(group => new
                {
                    UserId = group.Key.UserId,
                    UserName = group.Key.UserName,
                    UserEmail = group.Key.Email,
                    TestCount = group.Count(), // Count number of tests taken
                    UserTests = group.Select(ut => new
                    {
                        UserTestId = ut.Id, // UserTest Id
                        TestName = ut.Test.Name, // Test Name
                        DateTaken = ut.Date, // Date when the test was taken
                        Result = ut.Result // Result of the test
                    }).ToList() // List of tests for 
                })
                .ToListAsync();

            if (!userTestCounts.Any())
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
                Data = userTestCounts
            };
        }



        // End of GetAllUserTestsAsync
    }
}
