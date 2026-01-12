using System.Net.Http;
using System.Net.Http.Json;

namespace API
{
	public class AiAssistantClient
	{
		private readonly HttpClient _http;

		public AiAssistantClient(HttpClient http)
		{
			_http = http;
		}

		public async Task<AiResponse> GetHelpAsync(object payload)
		{
			var response = await _http.PostAsJsonAsync("/assist", payload);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadFromJsonAsync<AiResponse>();
		}
	}
}
