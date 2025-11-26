using System.Threading.Tasks;

namespace ChatBotApp.Services
{
    public interface IAzureOpenAIService
    {
        Task<string> GenerateResponseAsync(string userMessage);
    }
}