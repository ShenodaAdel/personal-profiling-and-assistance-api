using BusinessLogic.Services.ModelsAi.Dtos;

namespace BusinessLogic.Services.ModelsAi
{
    public interface IModelsIntegration
    {
        Task<string> AnalyzeAudioAsync(ModelDto dto);
        Task<string> AnalyzeImageAsync(ModelImageDto dto);
    }
}
