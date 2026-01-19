using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API.Interfaces
{
	public interface IImageService
	{
		Task<string> GetEnhancedBedroomImageUrl(string publicId);
		Task<List<string>> ExtractObjectsFromImage(string publicId, List<string> objects);
		Task<ImageUploadResult> UploadAndExtractObjects(IFormFile file, List<string> objects);
	}
}
