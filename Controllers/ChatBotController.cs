using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LittleHairSalon.Controllers
{
    [Route("Chatbot")]
    public class ChatbotController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public ChatbotController(IConfiguration config, IHttpClientFactory httpFactory)
        {
            _config = config;
            _http = httpFactory.CreateClient();
        }

        [HttpPost("Chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest req)
        {
            var apiKey = _config["Groq:ApiKey"];
            var model = _config["Groq:Model"] ?? "llama3-8b-8192";

            var systemPrompt = @"Bạn là trợ lý ảo của LittleHairSalon - salon tóc chuyên nghiệp.
Nhiệm vụ của bạn là:
- Tư vấn các dịch vụ: cắt tóc nam/nữ, uốn, nhuộm, gội đầu dưỡng sinh
- Giới thiệu sản phẩm: dầu gội, dầu xả, sáp vuốt tóc, thuốc nhuộm
- Hỗ trợ đặt lịch, báo giá dịch vụ
- Trả lời ngắn gọn, thân thiện bằng tiếng Việt
- Nếu khách muốn đặt lịch, hướng dẫn vào trang /DatLich";

            var body = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = req.Message }
                },
                max_tokens = 500,
                temperature = 0.7
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return Json(new { reply = $"Lỗi {response.StatusCode}: {err}" });
            }

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var reply = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Json(new { reply });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = "";
    }
}