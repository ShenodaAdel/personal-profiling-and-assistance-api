using BusinessLogic.DTOs;
using BusinessLogic.Services.Question;
using BusinessLogic.Services.Question.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Profiling_And_Assistance.Controllers.Admin.Questions
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion(int testId , [FromBody] QuestionAddDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid request data."
                });
            }

            var result = await _questionService.AddQuestionAsync(testId , dto);

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
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionAddWithChoicesDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid request data."
                });
            }

            var result = await _questionService.EditQuestionWithChoicesAsync(id, dto);

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

        [HttpPost("AddQustionWithChoice/{testId}")]
        public async Task<IActionResult> AddQuestionWithChoices( int testId , [FromBody] QuestionAddWithChoicesDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new ResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid input data."
                });
            }

            // Assuming your service method is in a class that implements the IYourService interface
            var result = await _questionService.AddQuestionWithChoicesAsync(testId, dto);

            if (result.Success)
            {
                return Ok(result); // Success - Return a 200 response with the result
            }
            else
            {
                return BadRequest(result); // Failure - Return a 400 response with the error message
            }
        }
    }
}
