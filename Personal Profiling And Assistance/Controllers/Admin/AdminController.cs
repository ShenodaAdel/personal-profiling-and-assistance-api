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
        private readonly ITestService _testService;
        private readonly IUserTestService _userTestService;
        private readonly IQuestionService _questionService;
        private readonly IChoiceService _choiceService;
        private readonly IQuestionChoiceService _questionChoiceService;
        // Admin Controller to USer 

        public AdminController(IUserService userService, ITestService testService, IUserTestService userTestService, IQuestionService questionService, IChoiceService choiceService , IQuestionChoiceService questionChoiceService)
        {
            _userService = userService;
            _testService = testService;
            _userTestService = userTestService;
            _questionService = questionService;
            _choiceService = choiceService;
            _questionChoiceService = questionChoiceService;
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

        // Admin Controller to Test

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
        public async Task<ActionResult<ResultDto>> GetTestsByUserId(int userId)
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

        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion([FromBody] QuestionAddDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid request data."
                });
            }

            var result = await _questionService.AddQuestionAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpGet("GetAllQuestion")]
        public async Task<IActionResult> GetAllQuestions()
        {
            var result = await _questionService.GetAllQuestionsAsync();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetQuestionById/{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var result = await _questionService.GetQuestionByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result); // 404 إذا لم يتم العثور على السؤال
            }

            return Ok(result); // 200 عند النجاح
        }

        [HttpPut("UpdateQuestion/{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionAddDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid request data."
                });
            }

            var result = await _questionService.UpdateQuestionAsync(id, dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteQuestion/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var result = await _questionService.DeleteQuestionAsync(id);

            if (!result.Success)
            {
                return NotFound(result); // 404 إذا لم يتم العثور على السؤال
            }

            return Ok(result); // 200 عند النجاح
        }

        [HttpPost("AddChoice")]
        public async Task<IActionResult> AddChoice([FromBody] ChoiceAddDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid request data."
                });
            }

            var result = await _choiceService.AddChoiceAsync(dto);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("DeleteChoice/{id}")]
        public async Task<IActionResult> DeleteChoice(int id)
        {
            var result = await _choiceService.DeleteChoiceAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("GetChoice/{id}")]
        public async Task<IActionResult> GetChoiceById(int id)
        {
            var result = await _choiceService.GetChoiceByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("GetAllChoices")]
        public async Task<IActionResult> GetAllChoices()
        {
            var result = await _choiceService.GetAllChoicesAsync();
            return Ok(result);
        }

        [HttpPut("UpdateChoice/{id}")]
        public async Task<IActionResult> UpdateChoice(int id, [FromBody] ChoiceAddDto dto)
        {
            var result = await _choiceService.UpdateChoiceAsync(id, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("AddQuestionChoice")]
        public async Task<IActionResult> AddQuestionChoice([FromBody] QuestionChoiceDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new ResultDto { Success = false, ErrorMessage = "Invalid request data." });
                }

                var result = await _questionChoiceService.AddQuestionChoiceAsync(dto);

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

        [HttpGet("GetAllQuestionChoices")]
        public async Task<IActionResult> GetAllQuestionChoices()
        {
            var result = await _questionChoiceService.GetAllQuestionChoicesAsync();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetQuestionChoiceById/{id}")]
        public async Task<IActionResult> GetQuestionChoiceById(int id)
        {
            var result = await _questionChoiceService.GetQuestionChoiceByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateQuestionChoice/{id}")]
        public async Task<IActionResult> UpdateQuestionChoice(int id, [FromBody] QuestionChoiceDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid request data."
                });
            }

            var result = await _questionChoiceService.UpdateQuestionChoiceAsync(id, dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteQuestionChoice/{id}")]
        public async Task<IActionResult> DeleteQuestionChoice(int id)
        {
            var result = await _questionChoiceService.DeleteQuestionChoiceAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}
