namespace API.Entities;

	public class ProfileImage
	{
		public int Id { get; set; }
		public string FileName { get; set; }
		public byte[] ImageData { get; set; }
		public byte[] ThumbnailData { get; set; }
		public string ContentType { get; set; }
		public string OriginalContentType { get; set; }
		public long FileSize { get; set; }
		public long OriginalFileSize { get; set; }
		public DateTime UploadedAt { get; set; }
		public int? AppUserId { get; set; }
		public AppUser AppUser { get; set; }
	}


