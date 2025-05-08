using BusinessLogic.Services.ModelsAi;
using BusinessLogic.Services.ModelsAi.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace Personal_Profiling_And_Assistance.Controllers.ModelsIntegration
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelIntegrationController : ControllerBase
    {
        private readonly IModelsIntegration _modelsIntegration;
        public ModelIntegrationController(IModelsIntegration modelsIntegration)
        {
            _modelsIntegration = modelsIntegration;
        }
        [HttpPost("analyze-audio")]
        public async Task<IActionResult> AnalyzeAudio([FromForm] ModelDto dto)
        {
            if (dto == null || dto.Voice.Length == 0)
                return BadRequest("Audio file is required.");
            var result = await _modelsIntegration.AnalyzeAudioAsync(dto);
            return Ok(result);
        }
        [HttpPost("analyze-image")]
        public async Task<IActionResult> AnalyzeImage([FromForm] ModelImageDto dto )
        {
            try
            {
                var emotion = await _modelsIntegration.AnalyzeImageAsync(dto);
                return Ok(emotion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
