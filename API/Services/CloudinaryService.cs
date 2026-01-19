using API.Helpers;

namespace API.Services
{
	using CloudinaryDotNet;
	using CloudinaryDotNet.Actions;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Options;
	using System.Runtime;
	using System.Security.Principal;

	public class CloudinaryService :IImageService
	{
		private readonly Cloudinary _cloudinary;
		private readonly CloudinarySettings _settings;
		private readonly string _folderName;
		private readonly IConfiguration _configuration;
		public CloudinaryService(Cloudinary cloudinary, IOptions<CloudinarySettings> config, IConfiguration configuration)
		{
			_settings = config.Value;
			_configuration = configuration;

			if (string.IsNullOrWhiteSpace(_settings.CloudName))
				throw new ArgumentException("CloudName does not match or is Incorrect");
			var acc = new Account(
				_settings.CloudName,
				_settings.ApiKey ?? string.Empty,
				_settings.ApiSecret ?? string.Empty
				);
			_cloudinary = new Cloudinary(acc);
			_cloudinary.Api.Secure = true;
			_folderName = _configuration["CloudinarySettings:Folder"];

		}

		public async Task<List<string>> ExtractObjectsFromImage(string publicId, List<string> objects)
		{
			try
			{
				var objectsPrompt = string.Join(";", objects);

				// Build URL with extract effect and multiple_true
				var url = _cloudinary.Api.UrlImgUp
					.ResourceType("image")
					.Transform(new Transformation()
						.Effect($"extract:prompt_({objectsPrompt});multiple_true"))
					.BuildUrl(publicId);

				// Since multiple_true returns multiple images, we need to construct URLs for each
				var extractedUrls = new List<string>();

				// Cloudinary returns images with indices when multiple_true is used
				// Format: publicId_0, publicId_1, publicId_2, etc.
				for (int i = 0; i < objects.Count; i++)
				{
					var indexedPublicId = $"{publicId.Replace(".jpg", "")}_{i}";
					var extractedUrl = _cloudinary.Api.UrlImgUp
						.BuildUrl(indexedPublicId);

					extractedUrls.Add(extractedUrl);
				}

				return  extractedUrls;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error extracting objects: {ex.Message}", ex);
			}
		}

		// Enhanced bedroom image with improvements
		public Task<string> GetEnhancedBedroomImageUrl(string publicId)
		{
			
			try
			{
				var url = _cloudinary.Api.UrlImgUp
					.ResourceType("image")
					.Action("upload")
					.Transform(new Transformation()
							.Effect("improve:indoor:50")
							.Quality("auto:best")
							.FetchFormat("auto")
							.Width(1920)
							.Height(1080)
							.Crop("fill")
							.Gravity("auto")
							)
					.BuildUrl(publicId);
				//https://res.cloudinary.com/dgooj7git/image/upload/e_improve:indoor:50/q_auto:best,f_auto,w_1920,h_1080,c_fill,g_auto/bedroom_a6l7g8.jpg';
				return Task.FromResult(url);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error enhancing image: {ex.Message}", ex);
			}
		}

		// Complete workflow: Upload, Extract, and Enhance
		public async Task<ImageUploadResult> UploadAndExtractObjects(
			IFormFile file,
			List<string> objects)
		{
			try
			{
				using var stream = file.OpenReadStream();

				var uploadParams = new ImageUploadParams
				{
					File = new FileDescription(file.FileName, stream),
					Folder = "Hotel assets",
					Transformation = new Transformation()
						.Effect($"extract:prompt_({string.Join(";", objects)});multiple_true")
						.Chain()
						.Effect("improve:indoor"),
					Tags = "bedroom,extraction"
				};

				var uploadResult = await _cloudinary.UploadAsync(uploadParams);
				return uploadResult;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error uploading and extracting: {ex.Message}", ex);
			}
		}
		public string GetEnhancedBedroomImageSimple()
		{
			// Simple URL generation without SDK
			
			var publicId = "samples/bedroom"; // Your image public ID
			var cloudName = _settings.CloudName;
			return $"https://res.cloudinary.com/{cloudName}/image/upload/w_800,h_600,c_fill,q_auto,f_auto/{publicId}";
		}
	}
}
