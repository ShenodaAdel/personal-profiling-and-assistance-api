using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace BusinessLogic.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetUserIdFromToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentException("Token is null or empty.");

                // Create a JWT Security Token Handler
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                    throw new ArgumentException("The token is not in a valid JWT format.");
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                    throw new ArgumentException("Invalid token.");

                // Extract user ID claim (sub) or (userId)
                var userIdClaim = jsonToken.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.Sub || c.Type.Equals("userId", StringComparison.OrdinalIgnoreCase));

                if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
                    throw new InvalidOperationException("User ID claim not found in the token.");

                return userIdClaim.Value;
            }
            catch (Exception ex)
            {
                throw new Exception("Error extracting user ID from token: " + ex.Message);
            }
        }
    }  
}
