using BusinessLogic.Services.Interfaces;
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

namespace BusinessLogic.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;

        public UserService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<ResultDto> AddUserAsync(UserDto dto)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(dto.Name) ||
                string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))

            {
                return new ResultDto
                {
                    HasError = true,
                    ErrorMessage = "Name, Email and password are required fields",
                    NotFound = false
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
                        HasError = true,
                        ErrorMessage = "User with this email or phone already exists",
                        NotFound = false
                    };
                }

                // Map DTO to User entity
                var user = new User
                {
                    Name = dto.Name,
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
                        Result = user,
                        HasError = false,
                        NotFound = false
                    };
                }

                return new ResultDto
                {
                    HasError = true,
                    ErrorMessage = "Failed to save user",
                    NotFound = false
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    HasError = true,
                    ErrorMessage = ex.Message,
                    NotFound = false
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
                    HasError = true,
                    ErrorMessage = "User not found",
                    NotFound = true
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
                        Result = user,
                        HasError = false,
                        NotFound = false
                    };
                }

                return new ResultDto
                {
                    HasError = true,
                    ErrorMessage = "Failed to delete user",
                    NotFound = false
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    HasError = true,
                    ErrorMessage = ex.Message,
                    NotFound = false
                };
            }
        }

        // // End of DeleteUserByIdAsync method

        public async Task<ResultDto> GetAllUserAsync()
        {
            var users = await _context.Users.ToListAsync();

            if (users.Count == 0)
            {
                return new ResultDto
                {
                    HasError = false,
                    NotFound = true
                };
            }

            return new ResultDto
            {
                Result = users,
                HasError = false,
                NotFound = false
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
                    HasError = true,
                    ErrorMessage = "Invalid user ID. ID must be a positive number.",
                    NotFound = false
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
                        NotFound = true,
                        HasError = false,
                        ErrorMessage = $"User with ID {id} not found."
                    };
                }

                // Map to safe DTO (exclude password)
                var userResponse = new UserDto 
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Gender = user.Gender,
                    ProfilePicture = user.ProfilePicture
                };

                return new ResultDto
                {
                    Result = userResponse,
                    HasError = false,
                    NotFound = false
                };
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return new ResultDto
                {
                    HasError = true,
                    ErrorMessage = $"An error occurred: {ex.Message}",
                    NotFound = false
                };
            }
        }

        // End of GetByIdUserAsync method

        public async Task<ResultDto> UpdateUserAsync(int id, UserDto dto)
        {
            User? user = await _context.Users.Where(u => u.Id == id).SingleOrDefaultAsync();
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
                                Result = user,
                                NotFound = false,
                                HasError = false,
                            };
                        }

                        return new ResultDto
                        {
                            NotFound = false,
                            HasError = true,
                            ErrorMessage = "User Not Updated.",
                        };
                    }

                    return new ResultDto
                    {
                        NotFound = false,
                        HasError = true,
                        ErrorMessage = "Phone Number or Email Address are Already Exist",
                    };
                }
                catch (Exception ex)
                {
                    return new ResultDto
                    {
                        NotFound = false,
                        HasError = true,
                        ErrorMessage = ex.Message
                    };
                }
            }

            return new ResultDto
            {
                NotFound = true,
                HasError = false,
            };
        }

        // End of UpdateUserAsync method
    }
}
