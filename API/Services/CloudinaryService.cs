using API.Helpers;

namespace API.Services
{
	using CloudinaryDotNet;
	using CloudinaryDotNet.Actions;
	using Microsoft.Extensions.Options;
	using System.Runtime;

	public class CloudinaryService
	{
		private readonly Cloudinary _cloudinary;
		private readonly CloudinarySettings _settings;
		//private readonly IConfiguration _config;
		public CloudinaryService(Cloudinary cloudinary, IOptions<CloudinarySettings> config)
		{
			_settings = config.Value;

			if (string.IsNullOrWhiteSpace(_settings.CloudName))
				throw new ArgumentException("CloudName does not match or is Incorrect");
			var acc = new Account(
				_settings.CloudName,
				_settings.ApiKey ?? string.Empty,
				_settings.ApiSecret ?? string.Empty
				);
			_cloudinary = new Cloudinary(acc);

		}

		public string GetEnhancedBedroomImage()
		{
			var publicId = "samples/enhancedRoom";
			return _cloudinary.Api.UrlImgUp
				.Transform(new Transformation()
					.Effect("gen_remove:background")
					.Effect("blur:200")
					.Effect("improve:indoor"))
				.BuildUrl("hotel/bedroom");
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
