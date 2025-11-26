using System;
using System.Threading.Tasks;

namespace ChatBotApp.Services
{
    public class AzureOpenAIService : IAzureOpenAIService
    {
        private readonly ILogger<AzureOpenAIService> _logger;
        private readonly IConfiguration _configuration;

        public AzureOpenAIService(IConfiguration configuration, ILogger<AzureOpenAIService> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> GenerateResponseAsync(string userMessage)
        {
            // Check if Azure OpenAI is configured
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var apiKey = _configuration["AzureOpenAI:ApiKey"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                _logger.LogInformation("Azure OpenAI not configured, using enhanced demo responses");
                return await Task.FromResult(GetEnhancedResponse(userMessage));
            }

            try
            {
                // TODO: Implement actual Azure OpenAI call when configuration is provided
                // For now, return enhanced responses with a note about configuration
                _logger.LogInformation("Azure OpenAI configured but implementation pending");
                return await Task.FromResult($"[Azure OpenAI Ready] {GetEnhancedResponse(userMessage)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error with Azure OpenAI service");
                return GetEnhancedResponse(userMessage);
            }
        }

        private string GetEnhancedResponse(string userMessage)
        {
            var lowerMessage = userMessage.ToLower();
            
            // Enhanced responses based on keywords
            if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi") || lowerMessage.Contains("hey"))
            {
                return "Hello! I'm your AI assistant. How can I help you today? Feel free to ask me anything!";
            }
            
            if (lowerMessage.Contains("help"))
            {
                return "I'm here to help! You can ask me questions about various topics, get information, or just have a conversation. What would you like assistance with?";
            }
            
            if (lowerMessage.Contains("weather"))
            {
                return "I'd love to help with weather information! While I don't have real-time weather data access in this demo, with Azure OpenAI integration, I could provide weather updates and forecasts.";
            }
            
            if (lowerMessage.Contains("time") || lowerMessage.Contains("date"))
            {
                return $"The current time is {DateTime.Now:HH:mm} and today's date is {DateTime.Now:MMMM dd, yyyy}. Is there anything else you'd like to know?";
            }
            
            if (lowerMessage.Contains("thank"))
            {
                return "You're very welcome! I'm glad I could help. Feel free to ask if you have any other questions!";
            }
            
            if (lowerMessage.Contains("name"))
            {
                return "I'm your ChatBot AI assistant! I'm here to help answer questions and have conversations. What's your name?";
            }
            
            if (lowerMessage.Contains("azure") || lowerMessage.Contains("openai"))
            {
                return "Great question about Azure OpenAI! This chatbot is designed to integrate with Azure OpenAI services for advanced AI capabilities. Configure your Azure OpenAI settings to unlock the full potential!";
            }

            // Default responses with variety
            var responses = new[]
            {
                $"That's an interesting question about '{userMessage}'. I'd love to provide a more detailed response with Azure OpenAI integration!",
                $"I understand you're asking about '{userMessage}'. With full AI capabilities, I could give you comprehensive information on this topic.",
                $"Thanks for your question: '{userMessage}'. This is exactly the type of query where Azure OpenAI would shine in providing detailed, contextual responses.",
                $"I appreciate you asking about '{userMessage}'. While I can provide basic responses now, Azure OpenAI would enable me to give much more insightful answers.",
                $"Regarding '{userMessage}' - that's a great topic to explore! Azure OpenAI integration would allow me to provide in-depth analysis and helpful insights."
            };

            var random = new Random();
            return responses[random.Next(responses.Length)];
        }
    }
}