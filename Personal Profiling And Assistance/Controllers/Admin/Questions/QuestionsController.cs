using BusinessLogic.DTOs;
using BusinessLogic.Services.Question;
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
    }
}
