using BusinessLogic.DTOs;
using BusinessLogic.Services.Choice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Profiling_And_Assistance.Controllers.Admin.Choices
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoicesController : ControllerBase
    {
        private readonly IChoiceService _choiceService;

        public ChoicesController(IChoiceService choiceService)
        {
            _choiceService = choiceService;
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

    }
}
