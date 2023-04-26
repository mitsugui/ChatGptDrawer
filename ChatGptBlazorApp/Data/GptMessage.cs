using System.Text.Json.Serialization;

namespace ChatGptBlazorApp.Data
{
	public class GptMessage
	{
		[JsonPropertyName("role")]
		public string Role { get; init; } = "user";

		[JsonPropertyName("content")]
		public string? Content { get; init; }
	}
}
