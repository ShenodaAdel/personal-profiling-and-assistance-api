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
        // Admin Controller to USer 

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

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetByIdUserAsync(id);

            if (!result.Success)
            {
                return NotFound(result); // 404 Not Found if user does not exist
            }

            return Ok(result); // 200 OK if user is found
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result); // Returns 404 if user not found
            }

            return Ok(result); // Returns 200 if deletion is successful
        }

        [HttpPut("UpdateAdmin/{id}")]
        public async Task<IActionResult> UpdateAdmin(string id, [FromForm] UpdateUserDto dto) // ✅ Use [FromForm]
        {
            if (string.IsNullOrWhiteSpace(id) || dto == null)
            {
                return BadRequest(new { Success = false, ErrorMessage = "Invalid request data." });
            }

            byte[]? profilePictureBytes = null;
            if (dto.ProfilePicture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.ProfilePicture.CopyToAsync(memoryStream);
                    profilePictureBytes = memoryStream.ToArray(); // ✅ Convert to byte[]
                }
            }

            var result = await _userService.UpdateUserAsync(id, dto.UserName, dto.Email, dto.PhoneNumber, dto.Gender, profilePictureBytes); // ✅ Send byte[] instead of IFormFile

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        // Admin Controller to USer

        // Admin Controller to Test
    }
}
