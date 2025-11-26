using Microsoft.AspNetCore.Mvc;
using ChatBotApp.Services;
using System.Threading.Tasks;

namespace ChatBotApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly IAzureOpenAIService _azureOpenAIService;

        public ChatController(IAzureOpenAIService azureOpenAIService)
        {
            _azureOpenAIService = azureOpenAIService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(message?.Message))
            {
                return BadRequest(new { response = "Message cannot be empty." });
            }

            try
            {
                var response = await _azureOpenAIService.GenerateResponseAsync(message.Message);
                return Json(new { response = response });
            }
            catch (Exception)
            {
                // Log error here
                return Json(new { response = "I'm sorry, I'm experiencing technical difficulties. Please try again later." });
            }
        }
    }

    public class ChatMessage
    {
        public string Message { get; set; } = string.Empty;
    }
}
