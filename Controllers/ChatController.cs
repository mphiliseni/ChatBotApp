using Microsoft.AspNetCore.Mvc;

namespace ChatBotApp.Controllers
{
    public class ChatController : Controller
    {
        [HttpPost]
        public IActionResult SendMessage(string message)
        {
            // Here you would implement logic to process the user's message and return a response.
            string botResponse = $"Bot response to '{message}'";
            return Json(new { response = botResponse });
        }
    }
}
