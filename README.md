# ChatBotApp

ChatBotApp is a basic ASP.NET Core MVC web application that integrates a live chatbot feature on the homepage. This chatbot allows users to interact by typing messages and receiving automated responses.
Features

    Chat Interface: A simple chatbot interface with an input field and response area.
    Responsive Design: The chatbot is displayed in a pop-up panel that can be toggled open and closed.
    ASP.NET Core MVC: Built using the Model-View-Controller architecture of ASP.NET Core.
    User Authentication: Includes options for users to register and log in.

# Technologies Used

    ASP.NET Core MVC
    HTML/CSS and JavaScript
    Bootstrap for styling


    ![ChatBot](https://github.com/user-attachments/assets/eb20e0fe-a13a-4693-b8e6-e0d5bd54fa5f)


# To integrate Azure OpenAI with your chatbot in the ChatBotApp project, you'll need to use Azure OpenAI's API to handle the conversation responses.

       Store Azure OpenAI Keys in appsettings.json

     Add your OpenAI ApiKey and Endpoint to your appsettings.json file:

     {
       "AzureOpenAI": {
        "ApiKey": "YOUR_API_KEY",
        "Endpoint": "https://YOUR_ENDPOINT_URL",
        "DeploymentId": "YOUR_DEPLOYMENT_ID"  // The ID of the deployed model
     }
    }

  # Note: Replace YOUR_API_KEY, YOUR_ENDPOINT_URL, and YOUR_DEPLOYMENT_ID with your actual Azure OpenAI key, endpoint, and model deployment ID.

  Create a Service to Call Azure OpenAI
      In  ChatBotApp project, create a new service AzureOpenAIService.cs to handle requests to the Azure OpenAI API.
      
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    
    public class AzureOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly string _deploymentId;

    public AzureOpenAIService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["AzureOpenAI:ApiKey"];
        _endpoint = configuration["AzureOpenAI:Endpoint"];
        _deploymentId = configuration["AzureOpenAI:DeploymentId"];
    }

    public async Task<string> GetChatbotResponseAsync(string userMessage)
    {
        var requestUrl = $"{_endpoint}/openai/deployments/{_deploymentId}/completions?api-version=2023-06-01";

        var requestBody = new
        {
            prompt = userMessage,
            max_tokens = 50,  // Adjust as needed
            temperature = 0.7
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync(requestUrl, requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);
        return responseJson.GetProperty("choices")[0].GetProperty("text").GetString();
     }
    }

  # Register the Service in Startup.cs or Program.cs

  In Program.cs, register the AzureOpenAIService so it can be injected into your controllers.
  
     builder.Services.AddHttpClient<AzureOpenAIService>();
     builder.Services.Configure<AzureOpenAIService>(builder.Configuration.GetSection("AzureOpenAI"));
  
# Integrate with Your ChatBot Controller

In your ChatBot controller, inject AzureOpenAIService and use it to fetch responses from Azure OpenAI.


    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    
    public class ChatBotController : Controller
    {
        private readonly AzureOpenAIService _azureOpenAIService;

    public ChatBotController(AzureOpenAIService azureOpenAIService)
    {
        _azureOpenAIService = azureOpenAIService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string userMessage)
    {
        if (string.IsNullOrEmpty(userMessage))
        {
            return Json(new { response = "Please enter a message." });
        }

        string botResponse = await _azureOpenAIService.GetChatbotResponseAsync(userMessage);
        return Json(new { response = botResponse });
     }
    }


# Update the Frontend (JavaScript)

Update your JavaScript code to call the /ChatBot/SendMessage endpoint when the user sends a message.

    <script>
        document.getElementById("sendButton").addEventListener("click", async function () {
            const userMessage = document.getElementById("userMessage").value;
    
            if (userMessage.trim() === "") return;
    
            const response = await fetch('/ChatBot/SendMessage', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ userMessage })
            });
    
            const data = await response.json();
            document.getElementById("chatWindow").innerHTML += `<div><strong>You:</strong> ${userMessage}</div>`;
            document.getElementById("chatWindow").innerHTML += `<div><strong>Bot:</strong> ${data.response}</div>`;
            document.getElementById("userMessage").value = "";
        });
    </script>


  #Test the Chatbot

    Run your application.
    Open the chatbot UI and type a message.
    Verify that the bot's response comes from Azure OpenAI.
