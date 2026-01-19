using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.Controllers
{
	public class ImageController : BaseApiController
	{
		
		private readonly CloudinaryService _cloudinaryService;


		public ImageController(CloudinaryService cloudinaryService)
		{
			_cloudinaryService = cloudinaryService;
		}

		[HttpGet("bedroom-enhanced")]	
		public async Task<IActionResult> GetBedroom()
		{
			var url = await _cloudinaryService.GetEnhancedBedroomImageUrl("bedroom_a6l7g8.jpg");
			using var httpClient = new HttpClient();

			try
			{
				//var response = await httpClient.GetAsync(url);
				//var statusCode = response.StatusCode;
				//var content = await response.Content.ReadAsStringAsync();
				//if(!response.IsSuccessStatusCode)
				//{
				//	throw new Exception($"Image fetch failed. Status: {(int)statusCode} {statusCode}. Response: {content}");
				//}

				var imageBytes = await httpClient.GetByteArrayAsync(url);
				return File(imageBytes, "image/jpeg");
			}
			catch (Exception ex)
			{

				return StatusCode(500, $"Error fetching image: {ex.Message },{ex.StackTrace}");
			}			
		}

		[HttpGet("Simple-Room")]
		public IActionResult GetSimpleRoom()
		{
			var url = _cloudinaryService.GetEnhancedBedroomImageSimple();
			return Ok(url);
		}
	}
}
