using BusinessLogic.DTOs;
using BusinessLogic.Services.Choice;
using BusinessLogic.Services.Question;
using BusinessLogic.Services.QuestionChoice;
using BusinessLogic.Services.QuestionChoice.Dtos;
using BusinessLogic.Services.Test;
using BusinessLogic.Services.User;
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

        // Admin Controller to USer

        // Admin Controller to Test
    }
}
