using BusinessLogic.DTOs;
using BusinessLogic.Services.UserTest;
using BusinessLogic.Services.UserTest.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Profiling_And_Assistance.Controllers.Admin.UserTest
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTestsController : ControllerBase
    {
        private readonly IUserTestService _userTestService;

        public UserTestsController(IUserTestService userTestService)
        {
            _userTestService = userTestService;
        }

        [HttpPost("AddTestToUser")]
        public async Task<ActionResult<ResultDto>> AddUserTestAsync([FromBody] UserTestDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new ResultDto { Success = false, ErrorMessage = "Invalid request data." });
                }

                var result = await _userTestService.AddUserTestAsync(dto);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultDto { Success = false, ErrorMessage = "An internal error occurred." });
            }
        }

        [HttpGet("GetTestsByUserId/{userId}")]
        public async Task<ActionResult<ResultDto>> GetTestsByUserId(string userId)
        {
            try
            {
                var result = await _userTestService.GetUserTestByIdAsync(userId);

                if (!result.Success || result.Data == null)
                {
                    return NotFound(new ResultDto
                    {
                        Success = false,
                        ErrorMessage = "No tests found for this user."
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultDto
                {
                    Success = false,
                    ErrorMessage = "An internal error occurred."
                });
            }
        }

        [HttpGet("GetAllUserTests")]
        public async Task<IActionResult> GetAllUserTests()
        {
            var result = await _userTestService.GetAllUserTestsAsync();

            if (!result.Success)
            {
                return NotFound(result); // 404 Not Found if no user tests exist
            }

            return Ok(result); // 200 OK with user test data
        }

        [HttpDelete("DeleteUserTest/{id}")]
        public async Task<IActionResult> DeleteUserTest(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid UserTest ID."
                });
            }

            var result = await _userTestService.DeleteUserTestAsync(id);

            if (!result.Success)
            {
                return NotFound(result); // Return 404 if not found
            }

            return Ok(result); // Return 200 if deletion was successful
        }

        [HttpPut("UpdateUserTest/{id}")]
        public async Task<IActionResult> UpdateUserTest(int id, [FromBody] UpdateUserTestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid request data."
                });
            }

            var result = await _userTestService.UpdateUserTestAsync(id, dto);

            if (!result.Success)
            {
                return BadRequest(result); // 400 Bad Request if update fails
            }

            return Ok(result); // 200 OK if update succeeds
        }
    }
}
