using BusinessLogic.DTOs;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using BusinessLogic.Services.User.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using BusinessLogic.Services.User.DTOs;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services.User
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private IConfiguration configuration;

        public UserService(MyDbContext context)
        {
            _context = context;
            _configuration = configuration;
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
                    u.Phone == dto.Phone);

                if (exists)
                {
                    return new ResultDto
                    {
                        Success = true,
                        ErrorMessage = "User with this email or phone already exists",
                    };
                }

                // Map DTO to User entity
                var user = new Data.Models.User
                {
                    Name = dto.Name,
                    Role = dto.Role,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Gender = dto.Gender,
                    ProfilePicture = dto.ProfilePicture,
                    Password = HashPassword(dto.Password) // Don't forget to hash the password!
                };

                await _context.Users.AddAsync(user);
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0)
                {
                    return new ResultDto
                    {
                        Data = user,
                        Success = true,
                    };
                }

                return new ResultDto
                {
                    Success = true,
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

        // Helper method for password hashing
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // End of AddUserAsync method

        public async Task<ResultDto> DeleteUserByIdAsync(int id)
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
                            user.Name,
                            user.Email,
                            user.Role
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
                u.Name,
                u.Email,
                u.Gender,
                u.ProfilePicture,
                u.Phone

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

        public async Task<ResultDto> GetByIdUserAsync(int id)
        {
            // Validate input
            if (id <= 0)
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
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Gender = user.Gender,
                    RoleManager = user.Role,
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

        public async Task<ResultDto> UpdateUserAsync(int id, Data.Models.User dto)
        {
            var user = await _context.Users.Where(u => u.Id == id).SingleOrDefaultAsync();
            if (user != null)
            {
                try
                {
                    // Check if the new phone number already exists in the database
                    bool PhoneCheck = await _context.Users.AnyAsync(u => u.Phone == dto.Phone);
                    bool EmailCheck = await _context.Users.AnyAsync(u => u.Email == dto.Email);

                    if (!PhoneCheck && !EmailCheck)
                    {
                        // Update the user fields with the new values from the dto
                        user.Name = dto.Name;
                        user.Phone = dto.Phone;
                        user.Email = dto.Email;
                        user.ProfilePicture = dto.ProfilePicture;
                        user.Password = HashPassword(dto.Password);
                        _context.Users.Update(user);

                        // Save changes to the database
                        int saveResult = await _context.SaveChangesAsync();
                        if (saveResult > 0)
                        {
                            return new ResultDto
                            {
                                Data = user,
                                Success = true,
                            };
                        }

                        return new ResultDto
                        {
                            Success = true,
                            ErrorMessage = "User Not Updated.",
                        };
                    }

                    return new ResultDto
                    {
                        Success = true,
                        ErrorMessage = "Phone Number or Email Address are Already Exist",
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

            return new ResultDto
            {
                Success = false,
            };
        }

        // End of UpdateUserAsync method

        public async Task<ResultDto> RegisterAsync(UserRegisterDto dto)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(dto.Name) ||
                string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return new ResultDto
                {
                    Success = false,
                    Data = null ,
                    Code = null ,
                    ErrorMessage = "Name, Email and password are required fields",
                };
            }
            try
            {
                // Check if email already exists
                var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
                if (exists)
                {
                    return new ResultDto
                    {
                        Success = false,
                        Data = null,
                        Code = null,
                        ErrorMessage = "User with this email already exists",
                    };
                }
                // Map DTO to User entity
                var user = new Data.Models.User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Role = "User",
                    Password = HashPassword(dto.Password) // Don't forget to hash the password!
                };
                await _context.Users.AddAsync(user);
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult > 0)
                {
                    return new ResultDto
                    {
                        Data = new
                        {
                            Name = user.Name,
                            Email = user.Email,
                            Role = user.Role
                        },
                        Success = true,
                        Code = null ,
                        ErrorMessage = null
                    };
                }
                return new ResultDto
                {
                    Success = false,
                    Data = null,
                    Code = null,
                    ErrorMessage = "Failed to save user",
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    Success = false,
                    Data = null,
                    Code = null,
                    ErrorMessage = ex.Message,
                };
            }
        }

        // End of RegisterAsync method

        public async Task<ResultDto> LoginAsync(UserLoginDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !VerifyPassword(dto.Password, user.Password))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password."
                };
            }

            var token = GenerateJwtToken(user);

            return new ResultDto
            {
                Success = true,
                Data = new { Token = token }
            };
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateJwtToken(Data.Models.User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)

            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
