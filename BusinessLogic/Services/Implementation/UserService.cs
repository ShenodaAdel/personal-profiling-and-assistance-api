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
                string.IsNullOrEmpty(dto.Email))
            {
                return new ResultDto
                {
                    HasError = true,
                    ErrorMessage = "Name, Email are required fields",
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
    }
}
