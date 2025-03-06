using BusinessLogic.DTOs;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.User
{
    public class GenerateTokenService : IGenerateTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject the IConfiguration and UserManager dependencies
        public GenerateTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        // Method to generate the JWT token
        public async Task<ResultDto> GenerateTokenAsync(string email, string password)
        {
            // Validate if the user exists
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
                // Fetch the JWT settings from configuration
                var jwtSettings = _configuration.GetSection("JWT");
                var issuer = jwtSettings.GetValue<string>("Issuer");
                var audience = jwtSettings.GetValue<string>("Audience");
                var secretKey = jwtSettings.GetValue<string>("SecretKey");
                var durationInDays = jwtSettings.GetValue<int>("DurationInDays");

                // Create the security key
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Create claims for the user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "User") // You can add roles here
                };

                // Create the token descriptor
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = issuer,
                    Audience = audience,
                    Expires = DateTime.UtcNow.AddDays(durationInDays),
                    SigningCredentials = credentials,
                    Subject = new ClaimsIdentity(claims),
                };

                // Create the JWT token handler and generate the token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Convert the token to string
                var tokenString = tokenHandler.WriteToken(token);

                // Return the token in a response DTO
                return new ResultDto
                {
                    Success = true,
                    Data = new { Token = tokenString }
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
    }
}
