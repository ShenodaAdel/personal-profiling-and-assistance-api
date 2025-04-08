using BusinessLogic.Services.Auth;
using BusinessLogic.Services.Auth.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Profiling_And_Assistance.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(IAuthService authService, RoleManager<IdentityRole> roleManager)
        {
            _authService = authService;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid registration data.");
            }

            var result = await _authService.RegisterAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid login data.");
            }
            var result = await _authService.LoginAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("LoginAdmin")]

        public async Task<IActionResult> LoginAdmin( LoginDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid login data.");
            }
            var result = await _authService.LoginAdminAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);

            if (result.Success)
            {
                return Ok(result); // Success response
            }
            else
            {
                return BadRequest(result); // Error response with message
            }
        }

        //[HttpGet("CreateRoles")]
        //public async Task<IActionResult> CreateRoles()
        //{
        //    // Check if the "Admin" role exists
        //    if (await _roleManager.RoleExistsAsync("Admin") == false)
        //    {
        //        // Create the "Admin" role
        //        var role = new IdentityRole("Admin");
        //        var result = await _roleManager.CreateAsync(role);
        //        if (result.Succeeded)
        //        {
        //        }
        //    }

        //    //// Check if the "User" role exists
        //    //if (await _roleManager.RoleExistsAsync("User") == false)
        //    //{
        //    //    // Create the "User" role
        //    //    var role = new IdentityRole("User");
        //    //    var result = await _roleManager.CreateAsync(role);
        //    //    if (result.Succeeded)
        //    //    {
        //    //    }
        //    //}

        //    // Return a view with the result message
        //    return RedirectToAction("Index", "Home");
        //}


    }
}
