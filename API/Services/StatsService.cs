using API.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace API.Services
{
	public class StatsService : IStatsService
	{
		private IHttpClientFactory _httpClient;
		private readonly IMemoryCache _stats;

		public StatsService(IHttpClientFactory httpClient, IMemoryCache stats)
		{
			_httpClient = httpClient;
			_stats = stats;
		}

		public async  Task<double> GetMaleFemaleRatio(string country)
		{
			var cacheKey = $"sex_ratio_{country}";

			if (_stats.TryGetValue(cacheKey, out double cachedRatio))
			{
				return cachedRatio;
			}
			using var httpClient = _httpClient.CreateClient("WorldBank");
			try
			{
			var countryCode = GetCountryCode(country); // e.g., "USA", "IND", "GBR"
	
				var maleUrl = $"https://api.worldbank.org/v2/country/{countryCode}/indicator/SP.POP.1564.TOTL.MA.IN?format=json&date=2022&countryCode={countryCode}";
				var femaleUrl = $"https://api.worldbank.org/v2/country/{countryCode}/indicator/SP.POP.1564.TOTL.FE.IN?format=json&date=2022&countryCode={countryCode}";

				var maleResponse = await httpClient.GetStringAsync(maleUrl);
				var femaleResponse = await httpClient.GetStringAsync(femaleUrl);

				var maleData = JsonConvert.DeserializeObject<JArray>(maleResponse);
				var femaleData = JsonConvert.DeserializeObject<JArray>(femaleResponse);
				var malePopulation = maleData[1].ToObject<List<WorldBankDataDto>>();
				var femalePopulation = femaleData[1].ToObject<List<WorldBankDataDto>>();
				if (maleData == null || femaleData == null || maleData?.Count == null || femaleData?.Count == null)
				{
					return 1.05;
				}
				if (femalePopulation[0].value == 0 || femalePopulation[0].value == null)
				{
					return 1.05;
				}
				else
				{
					var ratio = malePopulation[0].value / femalePopulation[0].value;

					_stats.Set(cacheKey, ratio, TimeSpan.FromHours(24));
					return ratio ?? 1.05;
				}
			}
			catch (Exception ex)
			{

				Console.WriteLine($"Failed to get ratio for {country}: {ex.Message}");
				return 1.05; 
			}
			
		}
		private string GetCountryCode(string country)
		{
			var codes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{ 
			{ "USA", "USA" }, { "United States", "USA" },
            { "India", "IND" }, { "IND", "IND" },
            { "China", "CHN" }, { "CHN", "CHN" },
            { "United Kingdom", "GBR" }, { "UK", "GBR" },
            { "Canada", "CAN" },
            { "Germany", "DEU" },
            { "France", "FRA" },
            { "Japan", "JPN" }
			};

			return codes.TryGetValue(country, out var code) ? code : country.ToUpper();
		}
	}
}
