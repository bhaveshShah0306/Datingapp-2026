using API.Services;
using Microsoft.AspNetCore.Mvc;

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
		public IActionResult GetBedroom()
		{
			var url = _cloudinaryService.GetEnhancedBedroomImage();
			return Ok(url);
		}

		[HttpGet("Simple-Room")]
		public IActionResult GetSimpleRoom()
		{
			var url = _cloudinaryService.GetEnhancedBedroomImageSimple();
			return Ok(url);
		}
	}
}
