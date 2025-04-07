using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTOs;
using BusinessLogic.Services.Auth.Dtos;
using BusinessLogic.Services.Emails;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Services.Auth
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailServices _emailService;
        public AuthService(UserManager<ApplicationUser> userManager, IEmailServices emailService, IConfiguration configuration , RoleManager<IdentityRole> roleManager , ILogger<AuthService>logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _logger = logger;
            _emailService = emailService;
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
            var token = await GenerateJwtToken(newUser);

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
                var token = await GenerateJwtToken(user);
                return new ResultDto
                {
                    Success = true,
                    Data = new { 
                        Token = token,
                        Roles = roles,
                        UserId = user.Id
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

        public async Task<ResultDto> LoginAdminAsync(LoginDto dto)
        {
            // Validate input
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Email and password are required"
                };
            }

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

                // ✅ Ensure only Admins can log in
                if (!roles.Contains("Admin"))
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "Access denied. Only Admins can log in."
                    };
                }

                // Generate JWT token for the Admin user
                var token = await GenerateJwtToken(user);
                if (string.IsNullOrEmpty(token))
                {
                    return new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "Token generation failed"
                    };
                }

                return new ResultDto
                {
                    Success = true,
                    Data = new
                    {
                        Token = token,
                        Roles = roles,
                        UserId = user.Id
                    }
                };
            }
            catch (Exception ex)
            {
                // Log the full exception details here
                _logger.LogError(ex, "Error during admin login");

                return new ResultDto
                {
                    Success = false,
                    ErrorMessage = "An error occurred during login"
                    // Consider not exposing full exception message in production
                };
            }
        }

        public async Task<ResultDto> ForgotPasswordAsync(ForgetPasswordDto dto)
        {
            var resultDto = new ResultDto();

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                resultDto.Success = true;
                return resultDto;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Encode token because it can have unsafe characters for URLs
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);

            // Build a reset URL (this should be front-end URL)
            var resetUrl = $"https://yourfrontend.com/reset-password?email={dto.Email}&token={encodedToken}";

            try
            {
                // Send the reset email
                await _emailService.SendResetPasswordEmailAsync(dto.Email, resetUrl);
                resultDto.Success = true;
                resultDto.Data = resetUrl;
            }
            catch (Exception ex)
            {
                // Log the error and return failure result
                _logger.LogError(ex, "Error sending reset password email");
                resultDto.Success = false;
                resultDto.ErrorMessage = "There was an error sending the password reset email.";
            }

            return resultDto;
        }
        public async Task<ResultDto> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var resultDto = new ResultDto();

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                resultDto.Success = false;
                resultDto.ErrorMessage = "Invalid request.";
                return resultDto;
            }

            // Decode token if needed
            var decodedToken = System.Web.HttpUtility.UrlDecode(dto.Token);

            var resetPassResult = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
            if (!resetPassResult.Succeeded)
            {
                resultDto.Success = false;
                resultDto.ErrorMessage = string.Join("; ", resetPassResult.Errors.Select(e => e.Description));
                return resultDto;
            }

            // Send confirmation email if reset is successful
            try
            {
                await _emailService.SendResetPasswordEmailAsync(dto.Email, "Your password has been successfully reset.");
                resultDto.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email after password reset.");
                resultDto.Success = true; // Still return success for password reset, but email failed
                resultDto.ErrorMessage = "Password reset successful, but there was an error sending the confirmation email.";
            }

            return resultDto;
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
                var token = await GenerateJwtToken(user);

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
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // 1. Read JWT settings (with validation)
            var secretKey = _configuration["JWT:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("JWT SecretKey is missing in configuration!");

            var issuer = _configuration["JWT:Issuer"] ?? "SecureApi";
            var audience = _configuration["JWT:Audience"] ?? "SecureApiUser";
            var durationInDays = _configuration.GetValue<int>("JWT:DurationInDays", 30); 

            // 2. Validate user properties
            if (string.IsNullOrEmpty(user.Id))
                throw new Exception("User Id is missing!");
            if (string.IsNullOrEmpty(user.Email))
                throw new Exception("User Email is missing!");

            // 3. Generate token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user) ?? new List<string>(); // Handle null roles

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName ?? "") // Handle null UserName
    };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(durationInDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
