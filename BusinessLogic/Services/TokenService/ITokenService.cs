using System;

namespace BusinessLogic.Services.TokenService
{
    public interface ITokenService
    {
       string GetUserIdFromToken(string Token);
    }
}
