using BusinessLogic.DTOs;
using BusinessLogic.Services.QuestionChoice;
using BusinessLogic.Services.QuestionChoice.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Profiling_And_Assistance.Controllers.Admin.QuestionChoices
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionChoicesController : ControllerBase
    {
        private readonly IQuestionChoiceService _questionChoiceService;

        public QuestionChoicesController(IQuestionChoiceService questionChoiceService)
        {
            _questionChoiceService = questionChoiceService;
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
