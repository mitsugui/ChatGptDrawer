using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace ChatGptBlazorApp.Data
{
	public class GptChatCompletionService
	{
		private readonly string _model;
		private readonly string _apiKey;
		private readonly HttpClient _httpClient = new();

		public GptChatCompletionService()
		{
			_apiKey = Environment.GetEnvironmentVariable("API_KEY")!;
            _model = "gpt-3.5-turbo";
        }

		public async Task<string> GetGpt4CompletionAsync(string prompt)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

			var apiUrl = "https://api.openai.com/v1/chat/completions";
			var requestBody = JsonSerializer.Serialize(new GptRequest
			{
				Model = _model,
				Messages = new[]
				{
					new GptMessage
					{
						Role = "system",
						Content = @"You are a coding machine that always returns a javascript function called start(), that can execute user input.
The user input is the description of a task that should be converted into running javascript code.
Your response must be only a valid javascript function called start() without any formatting eval()."
                    },
                    new GptMessage
                    {
                        Role = "user",
                        Content = @"Show the message 'Hello world!'."
                    },
                    new GptMessage
                    {
                        Role = "assistant",
                        Content = @"function start() { alert('Hello world!'); }"
                    },
                    new GptMessage
					{
						Role = "user",
						Content = prompt
					}
				}
			});

			using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
			using var responseMessage = await _httpClient.PostAsync(apiUrl, content);

			if (!responseMessage.IsSuccessStatusCode)
			{
				throw new Exception($"Error calling GPT-4 API: {responseMessage.ReasonPhrase}");
			}

			var responseBody = await responseMessage.Content.ReadAsStringAsync();
			var root = JsonSerializer.Deserialize<JsonElement>(responseBody);

            var choices = root.GetProperty("choices");
            var message = choices.EnumerateArray().First()
                .GetProperty("message");
            return message.GetProperty("content")
                .GetString() ?? "";

        }
	}
}
