using BusinessLogic.Services.ModelsAi.Dtos;
using Data;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Encodings.Web;
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
            if (dto == null || dto.Voice == null || dto.Voice.Length == 0)
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

                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                // English-to-Arabic translation
                var emotionTranslations = new Dictionary<string, string>
        {
            { "Angry", "غاضب" },
            { "Fearful", "خائف" },
            { "Happy", "سعيد" },
            { "Neutral", "طبيعي" },
            { "Sad", "حزين" },
            { "Surprised", "مندهش" }
        };

                if (result != null && result.TryGetValue("KeyVoice", out var emotionEn))
                {
                    var emotionAr = emotionTranslations.ContainsKey(emotionEn) ? emotionTranslations[emotionEn] : emotionEn;

                    var translatedResult = new Dictionary<string, string>
            {
                { "KeyVoice", emotionAr }
            };

                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false
                    };

                    return JsonSerializer.Serialize(translatedResult, options);
                }

                return json;
            }
            finally
            {
                System.IO.File.Delete(tempFilePath);
            }
        }

        public async Task<string> AnalyzeImageAsync(ModelImageDto dto)
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
                form.Add(new StreamContent(System.IO.File.OpenRead(tempFilePath)), "image", "photo.jpg");

                var response = await _httpClient.PostAsync("http://192.168.1.13:5000/predict_emotion", form);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                // Parse JSON
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                // English-to-Arabic translation
                var emotionTranslations = new Dictionary<string, string>
{
    { "Anger", "غضبان" },
    { "Happiness", "سعيد" },
    { "Sadness", "حزين" },
    { "Fear", "خائف" },
    { "Surprise", "مندهش" },
    { "Disgust", "مشمئز" },
    { "Neutral", "طبيعي" }
};

                if (result != null && result.TryGetValue("KeyImage", out var emotionEn))
                {
                    var emotionAr = emotionTranslations.ContainsKey(emotionEn) ? emotionTranslations[emotionEn] : emotionEn;

                    // Return new JSON with Arabic translation
                    var translatedResult = new Dictionary<string, string>
    {
        { "KeyImage", emotionAr }
    };

                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false
                    };

                    return JsonSerializer.Serialize(translatedResult, options);
                }

                return json;
            }
            finally
            {
                System.IO.File.Delete(tempFilePath);
            }
        }



    }


}
