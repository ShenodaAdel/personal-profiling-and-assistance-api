using BusinessLogic.DTOs;
using BusinessLogic.Services.Test.Dtos;
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

            // Validate if Description is not empty
            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test Description cannot be empty."
                };
            }

            // Validate if TestImage is provided
            if (dto.TestImage == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test Image is required."
                };
            }
            // Validate the file size (e.g., maximum 5 MB)
            if (dto.TestImage.Length > 5 * 1024 * 1024) // 5MB size limit
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test Image is too large. Maximum size is 5 MB."
                };
            }

            // Validate if the uploaded file is an image (basic check by file extension)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(dto.TestImage.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid file type. Allowed types are .jpg, .jpeg, .png, .gif."
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


            // Convert the image to a byte array
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await dto.TestImage.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            // If validations pass, add the new test
            var test = new Data.Models.Test
            {
                Name = dto.Name!,
                Description = dto.Description!,
                TestImage = imageBytes
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
                        test.Name,
                        test.Description,
                        test.TestImage
                    },
                    Success = true,
                    ErrorMessage = "Test is already Added."
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
                test.Description = dto.Description;

                // If a new TestImage is provided, process the image
                if (dto.TestImage != null)
                {
                    // Validate the image (size, type, etc.)
                    if (dto.TestImage.Length > 5 * 1024 * 1024) // 5 MB size limit
                    {
                        return new ResultDto
                        {
                            Success = false,
                            ErrorMessage = "Test Image is too large. Maximum size is 5 MB."
                        };
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(dto.TestImage.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return new ResultDto
                        {
                            Success = false,
                            ErrorMessage = "Invalid file type. Allowed types are .jpg, .jpeg, .png, .gif."
                        };
                    }

                    // Convert the image to a byte array
                    byte[] imageBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        await dto.TestImage.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }

                    // Update the TestImage field with the new image
                    test.TestImage = imageBytes;
                }

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
                t.Name,
                t.Description,
                t.TestImage
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
            var testsRaw = await _context.Tests.Select(t => new
            {
                t.Id,
                t.Name,
                QuestionCount  =  t.Questions.Count,
                t.Description,
                t.TestImage
            }).ToListAsync();

            // Step 2: Convert to DTOs in memory with image type detection
            var tests = testsRaw.Select(t => new
            {
                t.Id,
                t.Name,
                t.QuestionCount,
                t.Description,
                t.TestImage,
                ImageType = GetImageType(t.TestImage)
            }).ToList();



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

        public async Task<ResultDto> ViewTestAsync(int testId)
        {
            var test = await _context.Tests
                .Include(t => t.Questions)
                    .ThenInclude(q => q.QuestionChoices)
                    .ThenInclude(c => c.Choice)
                .FirstOrDefaultAsync(t => t.Id == testId);

            if (test == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Test not found."  // Detailed error message
                };
            }

            return new ResultDto
            {
                Success = true,
                Data = new ViewDto
                {
                    TestName = test.Name,
                    Questions = test.Questions.Select(q => new QuestionDto
                    {
                        QuestionId = q.Id,
                        Questioncontent = q.Content,
                        Choices = q.QuestionChoices.Select(qc => new ChoiceDto
                        {
                            ChoiceId = qc.Choice.Id,
                            ChoiceContent = qc.Choice.Content
                        }).ToList()
                    }).ToList()
                }
            };
        }

        private string GetImageType(byte[] imageData)
        {
            if (imageData == null || imageData.Length < 4)
                return "unknown";

            // PNG: 89 50 4E 47
            if (imageData[0] == 0x89 && imageData[1] == 0x50 && imageData[2] == 0x4E && imageData[3] == 0x47)
                return "png";

            // JPEG: FF D8 FF
            if (imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF)
                return "jpg";

            // GIF: 47 49 46
            if (imageData[0] == 0x47 && imageData[1] == 0x49 && imageData[2] == 0x46)
                return "gif";

            // BMP: 42 4D
            if (imageData[0] == 0x42 && imageData[1] == 0x4D)
                return "bmp";

            return "unknown";
        }
    }

}
