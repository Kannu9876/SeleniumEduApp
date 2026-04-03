using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SeleniumEduApp.Services
{
    public class GeminiAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _endpoint;

        public GeminiAssistantService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            
            // appsettings.json se key uthana
            _apiKey = config["GeminiApiKey"]; 

            // IMPORTANT: Hum "gemini-1.5-flash" use kar rahe hain jo ki latest stable model hai.
            // Agar aap "gemini-2.0-flash-exp" (experimental) use karna chahte hain toh wo bhi kar sakte hain.
            _endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
        }

        public async Task<string> GetDebugHelpAsync(string code, string errorMessage)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "⚠️ Error: API Key missing in appsettings.json!";
            }

            // AI ko context dena
            var requestBody = new
            {
                contents = new[]
                {
                    new 
                    { 
                        parts = new[] 
                        { 
                            new { text = $"You are a Selenium C# Expert. Fix this code.\nError: {errorMessage}\nCode: {code}" } 
                        } 
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_endpoint, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var jsonDocument = JsonDocument.Parse(responseString);
                    
                    // JSON se text nikalne ka sahi raasta
                    return jsonDocument.RootElement
                                  .GetProperty("candidates")[0]
                                  .GetProperty("content")
                                  .GetProperty("parts")[0]
                                  .GetProperty("text").GetString() ?? "AI returned empty response.";
                }
                
                // Agar abhi bhi NotFound aaye, toh yahan exact error dikhega
                return $"API Error: {response.StatusCode}. Please check if the Model Name in URL is correct.";
            }
            catch (Exception ex)
            {
                return $"Connection Error: {ex.Message}";
            }
        }
        // GeminiAssistantService.cs ke andar ye naya function jodein
public async Task<string> GetInterviewFeedbackAsync(string question, string userAnswer)
{
    string prompt = $"You are a Senior QA Manager. A candidate gave this answer to the question: '{question}'\n" +
                    $"Candidate's Answer: '{userAnswer}'\n" +
                    $"Please provide:\n1. A Score out of 10\n2. What was good\n3. What was missing\n4. The ideal answer they should have given.";

    var requestBody = new {
        contents = new[] {
            new { parts = new[] { new { text = prompt } } }
        }
    };

    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
    var response = await _httpClient.PostAsync(_endpoint, content);
    
    if (response.IsSuccessStatusCode)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(responseString);
        return jsonDocument.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
    }
    return "Error getting feedback.";
}
    }
}