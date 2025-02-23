using BusinessLogic.DTOs;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Test
{
    public class TestService : ITestService
    {
        private readonly MyDbContext _context;

        public TestService(MyDbContext context)
        {
            _context = context;
        } 

        public async Task<ResultDto> AddTestAsync(TestAddDto dto)
        {
            // Validate if Name is not empty
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return new ResultDto
                { 
                    Success = false,
                    ErrorMessage = "Test Name cannot be empty."
                };
            }

            // Validate if Name already exists in the database
            bool nameExists = await _context.Tests.AnyAsync(t => t.Name == dto.Name);
            if (nameExists)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test Name already exists in the database."
                };
            }

            // If validations pass, add the new test
            var test = new Data.Models.Test
            {
                Name = dto.Name
            };

            await _context.Tests.AddAsync(test);
            int saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                return new ResultDto
                {
                    Data = new
                    {
                        test.Id,
                        test.Name
                    },
                    Success = true
                };
            }

            return new ResultDto
            {
                Success = false,
                ErrorMessage = "Test could not be added."
            };
        }

        // End of AddTestAsync method

        public async Task<ResultDto> DeleteTestAsync(int id)
        {
            // Find the test by Id
            var test = await _context.Tests.FindAsync(id);

            // If the test doesn't exist, return an error
            if (test == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test not found."
                };
            }

            try
            {
                // Remove the test from the context
                _context.Tests.Remove(test);

                // Save changes to the database
                int saveResult = await _context.SaveChangesAsync();

                // If deletion is successful
                if (saveResult > 0)
                {
                    return new ResultDto
                    {
                        Success = true,
                        Data = new
                        {
                            test.Id,
                            test.Name
                        }
                    };
                }

                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test could not be deleted."
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

        // End of DeleteTestAsync method

        public async Task<ResultDto> UpdateTestAsync(int id, TestAddDto dto)
        {
            // Find the test by Id
            var test = await _context.Tests.FindAsync(id);

            // If the test doesn't exist, return an error
            if (test == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test not found."
                };
            }

            try
            {
                // Validate if Name is not empty
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "Test Name cannot be empty."
                    };
                }

                // Check if the Name already exists in the database (excluding the current test)
                bool nameExists = await _context.Tests.AnyAsync(t => t.Name == dto.Name && t.Id != id);
                if (nameExists)
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "Test Name already exists in the database."
                    };
                }

                // Update test properties from the DTO
                test.Name = dto.Name;
                _context.Tests.Update(test);
                int saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    return new ResultDto
                    {
                        Data = new
                        {
                            test.Id,
                            test.Name
                        },
                        Success = true
                    };
                }

                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "Test could not be updated."
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

        // End of UpdateTestAsync method

        public async Task<ResultDto> GetTestByIdAsync(int id)
        {
            // Find the test by Id
            var test = await _context.Tests
             .Where(t => t.Id == id)  // Filter by ID first
             .Select(t => new
             {
                t.Id,
                t.Name
             })
             .FirstOrDefaultAsync();

            // If the test doesn't exist, return an error
            if (test == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test not found."
                };
            }

            // Return the test if found
            return new ResultDto
            {
                Data = test,
                Success = true
            };
        }

        // End of GetTestByIdAsync method

        public async Task<ResultDto> GetAllTestsAsync()
        {
            // Retrieve all tests from the database
            var tests = await _context.Tests.Select(t => new
            {
                t.Id,
                t.Name
            }).ToListAsync();

            // Return an empty list with a success result if no tests are found
            return tests.Any()
                ? new ResultDto
                {
                    Data = tests,
                    Success = true
                }
                : new ResultDto
                {
                    Success = false,
                    ErrorMessage = "No tests found."
                };
        }

        // End of GetAllTestsAsync method


    }
}
