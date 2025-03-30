using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTOs;
using BusinessLogic.Services.Auth.Dtos;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task<ResultDto> RegisterAsync(RegisterDto dto)
        {
            var resultDto = new ResultDto();

            // Check if the user already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                resultDto.Success = false;
                resultDto.ErrorMessage = "User with this email already exists.";
                return resultDto;
            }

            // Create a new user (using email as username)
            var newUser = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email
            };

            var createUserResult = await _userManager.CreateAsync(newUser, dto.Password);
            if (!createUserResult.Succeeded)
            {
                resultDto.Success = false;
                resultDto.ErrorMessage = string.Join("; ", createUserResult.Errors.Select(e => e.Description));
                return resultDto;
            }

            // Ensure "User" role exists
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            var addRoleResult = await _userManager.AddToRoleAsync(newUser, "User");
            if (!addRoleResult.Succeeded)
            {
                resultDto.Success = false;
                resultDto.ErrorMessage = "User created, but role assignment failed: " +
                                         string.Join("; ", addRoleResult.Errors.Select(e => e.Description));
                return resultDto;
            }

            // Generate JWT token for the new user
            var token = GenerateJwtToken(newUser);

            // Wrap the token in a ResultDto and return it
            resultDto.Success = true;
            resultDto.Data = new AuthResponseDto
            { 
                Token = token,
                Role = "User"
            }; 
            // Ensure you wrap the token in an object if needed
            return resultDto;
        }

        public async Task<ResultDto> LoginAsync(LoginDto dto)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid credentials"
                };
            }

            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                // Generate JWT token for the valid user
                var token = GenerateJwtToken(user);
                return new ResultDto
                {
                    Success = true,
                    Data = new { 
                        Token = token,
                        Roles = roles
                    }
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

        public async Task<ResultDto> GenerateTokenAsync(string email, string password)
        {
            // Validate if the user exists and the password is correct
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid credentials"
                };
            }

            try
            {
                // Generate JWT token for the valid user
                var token = GenerateJwtToken(user);

                // Wrap the token in a ResultDto and return it
                return new ResultDto
                {
                    Success = true,
                    Data = new { Token = token }
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

        // Private helper method to generate a JWT token for a given user
        private string GenerateJwtToken(ApplicationUser user)
        {
            // Fetch the JWT settings from configuration
            var jwtSettings = _configuration.GetSection("JWT");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var durationInDays = jwtSettings.GetValue<int>("DurationInDays");

            // Create the security key and signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User") // Add additional roles if needed
            };

            // Create the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddDays(durationInDays),
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(claims)
            };

            // Generate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
