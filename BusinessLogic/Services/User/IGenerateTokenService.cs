using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.User
{
    public interface IGenerateTokenService
    {
        Task<ResultDto> GenerateTokenAsync(string email, string password);
    }
}
