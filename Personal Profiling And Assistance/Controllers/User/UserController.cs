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
        private readonly UserService _userService;
        public UserController(UserService userService)
        {  
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register( [FromBody] UserRegisterDto dto)
        {
            // Ensure the registration role is always "User" (no admin can register another user)
            var result = await _userService.RegisterAsync(dto);

            if (result.Success)
            {
                return Ok(result);  // Return success message if registration is successful
            }

            return BadRequest(result);  // Return error message if registration fails
        }

    }
}
