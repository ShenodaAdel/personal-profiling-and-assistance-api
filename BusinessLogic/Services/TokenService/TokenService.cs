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

                // Step 1: Create a JWT Security Token Handler
                var handler = new JwtSecurityTokenHandler();

                // Step 2: Parse the JWT Token
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                if (jsonToken == null)
                    throw new ArgumentException("Invalid token.");

                // Step 3: Extract user ID claim (sub) or (userId)
                var userIdClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == "userId");

                if (userIdClaim == null)
                    throw new ArgumentException("UserId claim not found in the token.");


                return userIdClaim.Value;
            }
            catch (Exception ex)
            {
                throw new Exception("Error extracting user ID from token: " + ex.Message);
            }
        }
    }  
}
