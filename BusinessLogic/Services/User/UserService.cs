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

        public UserService(MyDbContext context, UserManager<Data.Models.ApplicationUser> userManager, IConfiguration configuration, SignInManager<Data.Models.ApplicationUser> signManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signManager = signManager;
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
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "User not found",
                };
            }

            try
            {
                _context.Users.Remove(user);
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    return new ResultDto
                    {
                        Data = new
                        {
                            user.Id,
                            user.UserName,
                            user.Email
                        },
                        Success = true,
                    };
                }

                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Failed to delete user",
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

        // // End of DeleteUserByIdAsync method

        public async Task<ResultDto> GetAllUserAsync() 
        {
            var users = await _context.Users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,              
                u.PhoneNumber

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

        public async Task<ResultDto> UpdateUserAsync(string id, string? userName, string? email, string? phoneNumber, string? gender, byte[]? profilePicture)
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
                bool emailExists = await _context.Users.AnyAsync(u => u.Email == email && u.Id != id);

                if (phoneExists || emailExists)
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "Phone Number or Email Address already exists."
                    };
                }

                // ✅ Update user fields
                user.UserName = userName;
                user.PhoneNumber = phoneNumber;
                user.Email = email;
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
                            Email = user.Email,
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



        // End of UpdateUserAsync method



    }
}
