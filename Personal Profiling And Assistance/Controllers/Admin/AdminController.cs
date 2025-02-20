using BusinessLogic.DTOs;
using BusinessLogic.Services.Test;
using BusinessLogic.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Profiling_And_Assistance.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITestService _testService;
        

        public AdminController(IUserService userService , ITestService testService)
        {
            _userService = userService;
            _testService = testService;

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
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetByIdUserAsync(id);

            if (!result.Success)
            {
                return NotFound(result); // 404 Not Found if user does not exist
            }

            return Ok(result); // 200 OK if user is found
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result); // Returns 404 if user not found
            }

            return Ok(result); // Returns 200 if deletion is successful
        }

        
        [HttpPost("AddTest")]
        public async Task<IActionResult> AddTest([FromBody] TestAddDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid data."
                });
            }

            var result = await _testService.AddTestAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        
        [HttpGet("GetAllTests")]
        public async Task<IActionResult> GetAllTests()
        {
            var result = await _testService.GetAllTestsAsync();

            if (!result.Success)
            {
                return NotFound(result); // Return 404 if no tests are found
            }

            return Ok(result); // Return 200 with test data
        }

        
        [HttpGet("GetTestById/{id}")]
        public async Task<IActionResult> GetTestById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid test ID. ID must be a positive number."
                });
            }

            var result = await _testService.GetTestByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result); // Return 404 if the test is not found
            }

            return Ok(result); // Return 200 with the test data
        }

        [HttpPut("UpdateTest/{id}")]
        public async Task<IActionResult> UpdateTest(int id, [FromBody] TestAddDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid request data."
                });
            }

            var result = await _testService.UpdateTestAsync(id, dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteTest/{id}")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var result = await _testService.DeleteTestAsync(id);

            if (!result.Success)
            {
                return NotFound(result); // 404 if test not found
            }

            return Ok(result); // 200 if successfully deleted
        }



    }
}
