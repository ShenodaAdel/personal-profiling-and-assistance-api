using BusinessLogic.Services.ModelsAi.Dtos;
using Data;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Json;

namespace BusinessLogic.Services.ModelsAi
{
    public class ModelsIntegrationService : IModelsIntegration
    {
        private readonly MyDbContext _context;
        private readonly HttpClient _httpClient = new HttpClient();
        public ModelsIntegrationService(MyDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<string> AnalyzeAudioAsync(ModelDto dto)
        {
            if (dto == null || dto.Voice.Length == 0)
                throw new ArgumentException("Audio file is required.");

            var tempFilePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(tempFilePath))
            {
                await dto.Voice.CopyToAsync(stream);
            }

            try
            {
                using var form = new MultipartFormDataContent();
                form.Add(new StreamContent(System.IO.File.OpenRead(tempFilePath)), "file", "audio.wav");

                var response = await _httpClient.PostAsync("http://127.0.0.1:5000/predict", form);  
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                //var result = JsonSerializer.Deserialize<ResponseVoiceModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                //var emotion = result?.KeyVoice ?? "Unknown";

                return json;
            }
            finally
            {
                System.IO.File.Delete(tempFilePath);
            }
        }

        public async Task<string> AnalyzeImageAsync(ModelDto dto)
        {
            if (dto.Image == null || dto.Image.Length == 0)
                throw new ArgumentException("Image file is required.");

            var tempFilePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(tempFilePath))
            {
                await dto.Image.CopyToAsync(stream);
            }

            try
            {
                using var form = new MultipartFormDataContent();
                form.Add(new StreamContent(System.IO.File.OpenRead(tempFilePath)), "file", "image.jpg");

                var response = await _httpClient.PostAsync("http://127.0.0.1:5000/predict_emotion", form); // عدّل الرابط حسب API
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return json;
            }
            finally
            {
                System.IO.File.Delete(tempFilePath);
            }
        }


    }


}
