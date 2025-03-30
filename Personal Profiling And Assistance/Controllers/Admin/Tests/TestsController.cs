using BusinessLogic.DTOs;
using BusinessLogic.Services.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Personal_Profiling_And_Assistance.Controllers.Admin.TestController
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestsController(ITestService testService)
        {
            _testService = testService;
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


        [HttpGet("GetTestQuestions/{testId}")]
        public async Task<IActionResult> GetTestQuestions(int testId)
        {
            var result = await _testService.ViewTestAsync(testId);
            if (result == null)
            {
                return NotFound(new { message = "Test not found" });
            }
            return Ok(result);
        }

        // Admin Controller to Test
    }
}
