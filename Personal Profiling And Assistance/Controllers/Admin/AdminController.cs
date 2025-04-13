using BusinessLogic.DTOs;
using BusinessLogic.Services.Choice;
using BusinessLogic.Services.Question;
using BusinessLogic.Services.QuestionChoice;
using BusinessLogic.Services.QuestionChoice.Dtos;
using BusinessLogic.Services.Test;
using BusinessLogic.Services.User;
using BusinessLogic.Services.User.DTOs;
using BusinessLogic.Services.UserTest;
using BusinessLogic.Services.UserTest.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Personal_Profiling_And_Assistance.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IUserService _userService;


        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        //[Authorize(Roles = "Admin")] // Only admins can access this i need mena in this 
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUserAsync();

            if (!result.Success)
            {
                return NotFound(result); // 404 Not Found if no users exist
            }

            return Ok(result); // 200 OK with user list
        }

        [HttpGet("GetUserDetails/{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            var result = await _userService.GetByIdUserDetailsAsync(id);

            if (!result.Success)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpGet("GetUserById")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromHeader] string Authorization)
        {
            // Validate the Authorization header
            if (string.IsNullOrEmpty(Authorization) || !Authorization.StartsWith("Bearer "))
            {
                return Unauthorized(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Unauthorized: Missing or invalid token."
                });
            }
            var Token = Authorization.Substring("Bearer ".Length).Trim();
            var result = await _userService.GetByIdUserAsync(Token);

            if (!result.Success)
            {
                return NotFound(result); // 404 Not Found if user does not exist
            }

            return Ok(result); // 200 OK if user is found
        }

        [HttpDelete("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromHeader] string Authorization)
        {

            if (string.IsNullOrEmpty(Authorization) || !Authorization.StartsWith("Bearer "))
            {
                return Unauthorized(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Unauthorized: Missing or invalid token."
                });
            }
            var Token = Authorization.Substring("Bearer ".Length).Trim();
            var result = await _userService.DeleteUserByIdAsync(Token);

            if (!result.Success)
            {
                return NotFound(result); // Returns 404 if user not found
            }

            return Ok(result); // Returns 200 if deletion is successful
        }

        [HttpPut("UpdateAdmin")]
        [Authorize]
        public async Task<IActionResult> UpdateAdmin([FromHeader] string Authorization, [FromForm] UpdateUserDto dto) // ✅ Use [FromForm]
        {
            // Validate the Authorization header
            if (string.IsNullOrEmpty(Authorization) || !Authorization.StartsWith("Bearer "))
            {
                return Unauthorized(new { Success = false, ErrorMessage = "Unauthorized: Missing or invalid token." });
            }
            // Extract the token from the Authorization header (remove "Bearer " prefix)
            var Token = Authorization.Substring("Bearer ".Length).Trim();
            byte[]? profilePictureBytes = null;
            if (dto.ProfilePicture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.ProfilePicture.CopyToAsync(memoryStream);
                    profilePictureBytes = memoryStream.ToArray(); // ✅ Convert to byte[]
                }
            }

            var result = await _userService.UpdateUserAsync(Token, dto.UserName,dto.PhoneNumber, dto.Gender, profilePictureBytes); // ✅ Send byte[] instead of IFormFile

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("Analysis")]
        public async Task<IActionResult> GetAnalysis()
        {
            try
            {
                var result = await _userService.GetAnaiysisAsync();

                if (result.Success)
                {
                    return Ok(result);  
                }

                return BadRequest(result);  
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultDto
                {
                    Success = false,
                    ErrorMessage = $"An error occurred: {ex.Message}"
                });
            }
        }


        }
}
