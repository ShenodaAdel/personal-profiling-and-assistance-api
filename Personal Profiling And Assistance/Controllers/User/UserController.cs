using BusinessLogic.DTOs;
using BusinessLogic.Services.User;
using BusinessLogic.Services.User.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Profiling_And_Assistance.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {  
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = errorMessage ?? "Invalid request data"
                });
            }

            var result = await _userService.RegisterAsync(dto);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = errorMessage ?? "Invalid request data"
                });
            }

            var result = await _userService.LoginAsync(dto);

            if (result.Success)
            {
                return Ok(result); // 200 OK with token
            }

            return Unauthorized(result); // 401 Unauthorized for invalid credentials
        }


    }
}
