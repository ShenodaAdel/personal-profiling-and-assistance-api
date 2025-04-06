using BusinessLogic.DTOs;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.Services.User.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using BusinessLogic.Services.User.DTOs;

namespace BusinessLogic.Services.User
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Data.Models.ApplicationUser> _userManager;
        private readonly SignInManager<Data.Models.ApplicationUser> _signManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(MyDbContext context, UserManager<Data.Models.ApplicationUser> userManager, IConfiguration configuration, SignInManager<Data.Models.ApplicationUser> signManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
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

        public async Task<ResultDto> DeleteUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "User not found",
                };
            }

            // Delete related UserTests first
            var userTests = _context.UserTests.Where(ut => ut.UserId == id).ToList();
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
                    Data = new { user.Id, user.UserName, user.Email }
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
        public async Task<ResultDto> GetByIdUserAsync(string id)
        {
            // Validate input
            if (id == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid user ID. ID must be a positive number.",
                };
            }

            try
            {
                // Get user from database
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);

                // Handle not found
                if (user == null)
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = $"User with ID {id} not found."
                    };
                }

                // Map to safe DTO (exclude password)
                var userResponse = new 
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Gender = user.Gender,
                    ProfilePicture = user.ProfilePicture
                };

                return new ResultDto
                {
                    Data = userResponse,
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = $"An error occurred: {ex.Message}",
                
                };
            }
        }

        // End of GetByIdUserAsync method

        public async Task<ResultDto> UpdateUserAsync(string id, string? userName, string? phoneNumber, string? gender, byte[]? profilePicture)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
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
                bool phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != id);
                

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
                                Result = ut.Result
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

            // 3. Count UserTests for users who are not Admins
            var userTestCount = await _context.UserTests
                .Where(ut => !_context.UserRoles
                    .Any(ur => ur.UserId == ut.UserId && ur.RoleId == adminRoleId)) // Exclude Admins
                .CountAsync();

            // Return both counts
            return new ResultDto
            {
                Data = new { UserCount = userCount, UserTestCount = userTestCount },
                Success = true
            };
        }

    }

}
