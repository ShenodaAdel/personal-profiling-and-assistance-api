using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services.ModelsAi.Dtos
{
    public class ModelDto
    {
        public IFormFile? Voice { get; set; }

    }

    public class ResponseVoiceModel
    {
        public string? KeyVoice { get; set; }
    }

}
