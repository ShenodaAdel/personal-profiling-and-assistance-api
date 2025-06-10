using BusinessLogic.DTOs;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.Services.User.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using BusinessLogic.Services.Auth;
using System.IdentityModel.Tokens.Jwt;
using BusinessLogic.Services.TokenService;
using Newtonsoft.Json.Linq;


namespace BusinessLogic.Services.User
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Data.Models.ApplicationUser> _userManager;
        private readonly SignInManager<Data.Models.ApplicationUser> _signManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        public UserService(MyDbContext context, UserManager<Data.Models.ApplicationUser> userManager, IConfiguration configuration, SignInManager<Data.Models.ApplicationUser> signManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<ResultDto> AddUserAsync(UserAddDto dto)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(dto.Name) ||
                string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))

            {
                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = "Name, Email and password are required fields",
                };
            }

            try
            {
                // Check if email or phone already exists 
                var exists = await _context.Users.AnyAsync(u =>
                    u.Email == dto.Email ||
                    u.PhoneNumber == dto.Phone);

                if (exists)
                {
                    return new ResultDto
                    {
                        Success = true,
                        ErrorMessage = "User with this email or phone already exists",
                    };
                }

                // Map DTO to User entity
                var user = new ApplicationUser
                {
                    UserName = dto.Name,
                    Email = dto.Email,
                    PhoneNumber = dto.Phone,
                    Gender = dto.Gender,
                    ProfilePicture = dto.ProfilePicture
                };

                var saveResult  = await _userManager.CreateAsync(user, dto.Password);

                if (saveResult.Succeeded)
                {
                    return new ResultDto
                    {
                        Data = user,
                        Success = true,
                    };
                }

                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Failed to save user",                
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                };
            }
        }

        // End of AddUserAsync method

        public async Task<ResultDto> DeleteUserByTokenAsync(string Token)
        {

            var userId = _tokenService.GetUserIdFromToken(Token);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "User not found",
                };
            }

            // Delete related UserTests first
            var userTests = _context.UserTests.Where(ut => ut.UserId == userId).ToList();
            if (userTests.Any())
            {
                _context.UserTests.RemoveRange(userTests);
                await _context.SaveChangesAsync();
            }

            // Then delete the user
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return new ResultDto
                {
                    Success = true,
                    Data = new { user.Id, user.UserName, user.Email },
                    ErrorMessage = "User deleted successfully"
                };
            }

            return new ResultDto
            {
                Success = false,
                ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        // // End of DeleteUserByIdAsync method
        public async Task<ResultDto> GetAllUserAsync() 
        {
            var users = await _context.Users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,              
                u.PhoneNumber,
                u.ProfilePicture

            }).ToListAsync();

            if (users.Count == 0)
            {
                return new ResultDto
                {
                    Success = false,           
                };
            }

            return new ResultDto
            {
                Data = users,
                Success = true,
            };
        }

        // End of GetAllUserAsync method
        public async Task<ResultDto> GetByIdUserAsync(string token)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(token))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid token."
                };
            }

            try
            {
                // Get user ID from token
                var userId = _tokenService.GetUserIdFromToken(token);

                // First, retrieve the user data without the method calls in the projection
                var userEntity = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .Select(u => new
                    {
                        Id = u.Id,
                        Name = u.UserName,
                        Email = u.Email,
                        Phone = u.PhoneNumber,
                        Gender = u.Gender,
                        ProfilePicture = u.ProfilePicture,
                        Tests = u.UserTests
                            .OrderBy(ut => ut.Date)
                            .Select(ut => new
                            {
                                TestId = ut.TestId,
                                TestName = ut.Test.Name,
                                DateTaken = ut.Date,
                                Result = ut.Result // Get the raw result string
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (userEntity == null)
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "User not found."
                    };
                }

                // Process the data after fetching from the database
                var userDto = new
                {
                    userEntity.Id,
                    userEntity.Name,
                    userEntity.Email,
                    userEntity.Phone,
                    userEntity.Gender,
                    userEntity.ProfilePicture,
                    ProfilePictureType = GetImageType(userEntity.ProfilePicture),
                    Tests = userEntity.Tests.Select(t => new
                    {
                        t.TestId,
                        t.TestName,
                        t.DateTaken,
                        Result = ExtractValueFromJsonString(t.Result)
                    }).ToList()
                };

                return new ResultDto
                {
                    Data = userDto,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = $"An error occurred: {ex.Message}"
                };
            }
        }

        // End of GetByIdUserAsync method

        public async Task<ResultDto> UpdateUserAsync(string Token, string? userName, string? phoneNumber, string? gender, byte[]? profilePicture)
        {
            var userId = _tokenService.GetUserIdFromToken(Token);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };
            }

            try
            {
                bool phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != userId);
                

                if (phoneExists)
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "Phone Number already exists."
                    };
                }

                // ✅ Update user fields
                user.UserName = userName;
                user.PhoneNumber = phoneNumber;
                user.Gender = gender;

                if (profilePicture != null)
                {
                    user.ProfilePicture = profilePicture; // ✅ Store byte[] in database
                }

                _context.Users.Update(user);

                int saveResult = await _context.SaveChangesAsync();
                if (saveResult > 0)
                {
                    return new ResultDto
                    {
                        Success = true,
                        Data = new
                        {
                            Name = user.UserName,
                            Phone = user.PhoneNumber,
                            Gender = user.Gender,
                            ProfilePicture = user.ProfilePicture // ✅ Return byte[]
                        }
                    };
                }

                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "User update failed."
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ResultDto> GetByIdUserDetailsAsync(string id)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(id))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid user ID."
                };
            }

            try
            {
                // Get user from database along with their tests
                var user = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == id)
                    .Select(u => new
                    {
                        Id = u.Id,
                        Name = u.UserName,
                        Email = u.Email,
                        Phone = u.PhoneNumber,
                        Gender = u.Gender,
                        ProfilePicture = u.ProfilePicture,
                        Tests = _context.UserTests
                            .Where(ut => ut.UserId == u.Id)
                            .Select(ut => new
                            {
                                TestId = ut.TestId,
                                TestName = ut.Test!.Name, // Assuming "Name" exists in the Test entity
                                DateTaken = ut.Date,
                                Result = ExtractValueFromJsonString(ut.Result)
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                // Handle not found case
                if (user == null)
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = $"User with ID {id} not found."
                    };
                }

                return new ResultDto
                {
                    Data = user,
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = $"An error occurred: {ex.Message}"
                };
            }
        }

        // End of UpdateUserAsync method

        public async Task<ResultDto> GetAnaiysisAsync()
        {
            // 1. Get Admin Role Id
            var adminRole = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Admin role not found."
                };
            }

            var adminRoleId = adminRole.Id;

            // 2. Count users excluding Admins
            var userCount = await (from user in _userManager.Users
                                   where !_context.UserRoles
                                        .Any(ur => ur.UserId == user.Id && ur.RoleId == adminRoleId)
                                   select user)
                                   .CountAsync();


            var userTestCount = await _context.UserTests.CountAsync();

            var testCount = await _context.Tests.CountAsync();

            // 4. Get the count of user tests for each test in the system
            var testCounts = await _context.UserTests
                .Join(_context.Tests, ut => ut.TestId, t => t.Id, (ut, t) => new { ut, t })  // Join UserTests with Tests
                .GroupBy(x => x.t.Name)  // Group by Test Name
                .Select(group => new
                {
                    TestName = group.Key,  // Test Name
                    UserTestCount = group.Count(), // Count the number of user tests for this test
                    Ratio = (double)group.Count() / userTestCount * 100  
                })
                .ToListAsync();

            // 4. Retrieve all UserTests and perform the result extraction after fetching the data
            var userTests = await _context.UserTests
                .Include(ut => ut.Test) // Ensure that Test info is included
                .ToListAsync();

            // 5. For each test, count the occurrences of each result type ("طبيعي", "خفيف", "متوسط", "شديد")
            var resultCountsByTest = userTests
                .GroupBy(ut => ut.Test.Name)  // Group by Test Name
                .Select(group => new
                {
                    TestName = group.Key, // Test Name
                    TotalCount = group.Count(), // Total number of tests for this test
                    NaturalCount = group.Count(ut => ExtractValueFromJsonString(ut.Result) == "طبيعي"), // Count "طبيعي"
                    LightCount = group.Count(ut => ExtractValueFromJsonString(ut.Result) == "خفيف"), // Count "خفيف"
                    ModerateCount = group.Count(ut => ExtractValueFromJsonString(ut.Result) == "متوسط"), // Count "متوسط"
                    SevereCount = group.Count(ut => ExtractValueFromJsonString(ut.Result) == "شديد"), // Count "شديد"
                    NaturalRatio = (double)group.Count(ut => ExtractValueFromJsonString(ut.Result) == "طبيعي") / group.Count() * 100, // Calculate ratio for "طبيعي"
                    LightRatio = (double)group.Count(ut => ExtractValueFromJsonString(ut.Result) == "خفيف") / group.Count() * 100, // Calculate ratio for "خفيف"
                    ModerateRatio = (double)group.Count(ut => ExtractValueFromJsonString(ut.Result) == "متوسط") / group.Count() * 100, // Calculate ratio for "متوسط"
                    SevereRatio = (double)group.Count(ut => ExtractValueFromJsonString(ut.Result) == "شديد") / group.Count() * 100 // Calculate ratio for "شديد"
                })
                .ToList();

            // Return both counts
            return new ResultDto
            { 
                Data = new { 
                    UserCount = userCount, 
                    UserTestCount = userTestCount,
                    TestCount = testCount,
                    TestCounts = testCounts,
                    ResultCounts = resultCountsByTest
                },
                Success = true
            };
        }

        public static string ExtractValueFromJsonString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            if (input.StartsWith("key:", StringComparison.OrdinalIgnoreCase))
            {
                return input.Replace("key:", "", StringComparison.OrdinalIgnoreCase).Trim();
            }

            return input.Trim(); // لو مفيش key: رجع النص كما هو بعد إزالة المسافات
        }



        public async Task<ResultDto> DeleteUserByIdAsync(string userId)
        {
            var user = await _context.Users.Include(u => u.UserTests).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };
            }
            try
            {
                if (user.UserTests != null && user.UserTests.Any())
                {
                    _context.UserTests.RemoveRange(user.UserTests);
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return new ResultDto
                {
                    Success = true,
                    ErrorMessage = $"User deleted successfully."
                };
            }
            catch (Exception ex)
            {
                // Optionally: log the exception here
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = $"Failed to delete user: {ex.Message}"
                };
            }

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
