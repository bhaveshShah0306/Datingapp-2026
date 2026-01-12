namespace API.Helpers;

    public class CloudinarySettings
    {
        private static IConfiguration _config;
		private CloudinarySettings(IConfiguration config)
        {
           _config = config;
		}
		public string CloudName { get; init; }
        public string ApiKey { get; init; }
        public string ApiSecret { get; init; }
		public string Environment =>
		   System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
		public string DevEnvironmentVariable =>
			 System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;//System.Environment.GetEnvironmentVariable("DEV_CLOUDINARY_URL") ?? string.Empty;

}
