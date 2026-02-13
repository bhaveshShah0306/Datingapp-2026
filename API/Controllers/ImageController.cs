using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
	//private readonly SqlServerDbContext _sqlContext;
	private readonly DataContext _postgresContext;

	// Profile picture settings
	private const int MaxProfileWidth = 1024;
	private const int MaxProfileHeight = 1024;
	private const int ThumbnailSize = 256;

	public ImageController(
		//SqlServerDbContext sqlContext,
		DataContext postgresContext)
	{
		//_sqlContext = sqlContext;
		_postgresContext = postgresContext;
	}

	#region MS SQL Server Endpoints (commented out for now)

	///// <summary>
	///// Upload profile picture to SQL Server - Auto-resizes, supports HDR
	///// </summary>
	////[HttpPost("profile/sqlserver")]
	////public async Task<IActionResult> UploadProfileToSqlServer(IFormFile file)
	////{
	////	if (file == null || file.Length == 0)
	////		return BadRequest("No file uploaded");

	////	// Validate file type - including HDR formats
	////	var allowedTypes = new[]
	////	{
	////		"image/jpeg",
	////		"image/png",
	////		"image/gif",
	////		"image/webp",
	////		"image/heic",  // HDR iPhone images
	////           "image/heif",  // HDR format
	////           "image/avif"   // Modern HDR format
	////       };

	////	if (!allowedTypes.Contains(file.ContentType.ToLower()))
	////		return BadRequest($"Invalid file type: {file.ContentType}. Allowed types: JPEG, PNG, GIF, WebP, HEIC, HEIF, AVIF");

	////	try
	////	{
	////		// Process and resize image
	////		var (resizedImage, thumbnailImage) = await ProcessProfileImage(file);

	////		var image = new ProfileImage
	////		{
	////			FileName = file.FileName,
	////			ImageData = resizedImage,
	////			ThumbnailData = thumbnailImage,
	////			ContentType = "image/webp", // Convert to WebP for efficiency
	////			OriginalContentType = file.ContentType,
	////			FileSize = resizedImage.Length,
	////			OriginalFileSize = file.Length
	////		};

	////		_sqlContext.ProfileImages.Add(image);
	////		await _sqlContext.SaveChangesAsync();

	////		return Ok(new
	////		{
	////			id = image.Id,
	////			fileName = image.FileName,
	////			originalSize = FormatFileSize(image.OriginalFileSize),
	////			optimizedSize = FormatFileSize(image.FileSize),
	////			compressionRatio = $"{(1 - (double)image.FileSize / image.OriginalFileSize) * 100:F1}%",
	////			message = "Profile picture saved to SQL Server"
	////		});
	////	}
	////	catch (Exception ex)
	////	{
	////		return BadRequest($"Error processing image: {ex.Message}");
	////	}
	////}
	///
	////[HttpGet("profile/sqlserver/{id}")]
	////public async Task<IActionResult> GetProfileFromSqlServer(int id, [FromQuery] bool thumbnail = false)
	////{
	////	var image = await _sqlContext.ProfileImages.FindAsync(id);

	////	if (image == null)
	////		return NotFound();

	////	var imageData = thumbnail ? image.ThumbnailData : image.ImageData;
	////	return File(imageData, image.ContentType, image.FileName);
	////}

	////[HttpGet("profile/postgresql/{id}")]
	////public async Task<IActionResult> GetProfileFromPostgreSQL(int id, [FromQuery] bool thumbnail = false)
	////{
	////	var image = await _postgresContext.ProfileImages.FindAsync(id);

	////	if (image == null)
	////		return NotFound();

	////	var imageData = thumbnail ? image.ThumbnailData : image.ImageData;
	////	return File(imageData, image.ContentType, image.FileName);
	////}

	//#endregion

	//#region Regular Image Endpoints (with 5MB limit)

	////[HttpPost("sqlserver")]
	////public async Task<IActionResult> UploadToSqlServer(IFormFile file)
	////{
	////	if (file == null || file.Length == 0)
	////		return BadRequest("No file uploaded");

	////	var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
	////	if (!allowedTypes.Contains(file.ContentType.ToLower()))
	////		return BadRequest("Invalid file type");

	////	if (file.Length > 5 * 1024 * 1024)
	////		return BadRequest("File size exceeds 5MB limit");

	////	using var memoryStream = new MemoryStream();
	////	await file.CopyToAsync(memoryStream);
	////	byte[] imageBytes = memoryStream.ToArray();

	////	var image = new Image
	////	{
	////		FileName = file.FileName,
	////		ImageData = imageBytes,
	////		ContentType = file.ContentType,
	////		FileSize = imageBytes.Length
	////	};

	////	_sqlContext.Images.Add(image);
	////	await _sqlContext.SaveChangesAsync();

	////	return Ok(new
	////	{
	////		id = image.Id,
	////		fileName = image.FileName,
	////		fileSize = image.FileSize,
	////		message = "Image saved to SQL Server"
	////	});
	////}

	////[HttpGet("sqlserver/{id}")]
	////public async Task<IActionResult> GetFromSqlServer(int id)
	////{
	////	var image = await _sqlContext.Images.FindAsync(id);

	////	if (image == null)
	////		return NotFound();

	////	return File(image.ImageData, image.ContentType, image.FileName);
	////}

	////[HttpGet("sqlserver/{id}/info")]
	////public async Task<IActionResult> GetInfoFromSqlServer(int id)
	////{
	////	var image = await _sqlContext.Images
	////		.Where(i => i.Id == id)
	////		.Select(i => new
	////		{
	////			i.Id,
	////			i.FileName,
	////			i.ContentType,
	////			i.FileSize,
	////			i.UploadedAt
	////		})
	////		.FirstOrDefaultAsync();

	////	if (image == null)
	////		return NotFound();

	////	return Ok(image);
	////}

	////[HttpDelete("sqlserver/{id}")]
	////public async Task<IActionResult> DeleteFromSqlServer(int id)
	////{
	////	var image = await _sqlContext.Images.FindAsync(id);

	////	if (image == null)
	////		return NotFound();

	////	_sqlContext.Images.Remove(image);
	////	await _sqlContext.SaveChangesAsync();

	////	return NoContent();
	////}
	/// <summary>
	/// ////[HttpGet("sqlserver")]
	////public async Task<IActionResult> ListFromSqlServer()
	////{
	////	var images = await _sqlContext.Images
	////		.Select(i => new
	////		{
	////			i.Id,
	////			i.FileName,
	////			i.ContentType,
	////			i.FileSize,
	////			i.UploadedAt
	////		})
	////		.OrderByDescending(i => i.UploadedAt)
	////		.ToListAsync();

	////	return Ok(images);
	////}
	/// </summary>
	/// <param name="file"></param>
	/// <returns></returns>
	#endregion

	#region Profile Picture Endpoints (with resizing and HDR support)
	/// <summary>
	/// Upload profile picture to PostgreSQL - Auto-resizes, supports HDR
	/// </summary>
	[HttpPost("profile/postgresql")]
	public async Task<IActionResult> UploadProfileToPostgreSQL(IFormFile file)
	{
		if (file == null || file.Length == 0)
			return BadRequest("No file uploaded");

		var allowedTypes = new[]
		{
			"image/jpeg",
			"image/png",
			"image/gif",
			"image/webp",
			"image/heic",
			"image/heif",
			"image/avif"
		};

		if (!allowedTypes.Contains(file.ContentType.ToLower()))
			return BadRequest($"Invalid file type: {file.ContentType}");

		try
		{
			var (resizedImage, thumbnailImage) = await ProcessProfileImage(file);

			var image = new ProfileImage
			{
				FileName = file.FileName,
				ImageData = resizedImage,
				ThumbnailData = thumbnailImage,
				ContentType = "image/webp",
				OriginalContentType = file.ContentType,
				FileSize = resizedImage.Length,
				OriginalFileSize = file.Length
			};

			_postgresContext.ProfileImages.Add(image);
			await _postgresContext.SaveChangesAsync();

			return Ok(new
			{
				id = image.Id,
				fileName = image.FileName,
				originalSize = FormatFileSize(image.OriginalFileSize),
				optimizedSize = FormatFileSize(image.FileSize),
				compressionRatio = $"{(1 - (double)image.FileSize / image.OriginalFileSize) * 100:F1}%",
				message = "Profile picture saved to PostgreSQL"
			});
		}
		catch (Exception ex)
		{
			return BadRequest($"Error processing image: {ex.Message}");
		}
	}


	[HttpPost("postgresql")]
	public async Task<IActionResult> UploadToPostgreSQL(IFormFile file)
	{
		if (file == null || file.Length == 0)
			return BadRequest("No file uploaded");

		var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
		if (!allowedTypes.Contains(file.ContentType.ToLower()))
			return BadRequest("Invalid file type");

		if (file.Length > 5 * 1024 * 1024)
			return BadRequest("File size exceeds 5MB limit");

		using var memoryStream = new MemoryStream();
		await file.CopyToAsync(memoryStream);
		byte[] imageBytes = memoryStream.ToArray();

		var image = new ProfileImage
		{
			FileName = file.FileName,
			ImageData = imageBytes,
			ContentType = file.ContentType,
			FileSize = imageBytes.Length
		};

		_postgresContext.ProfileImages.Add(image);
		await _postgresContext.SaveChangesAsync();

		return Ok(new
		{
			id = image.Id,
			fileName = image.FileName,
			fileSize = image.FileSize,
			message = "Image saved to PostgreSQL"
		});
	}

	[HttpGet("postgresql/{id}")]
	public async Task<IActionResult> GetFromPostgreSQL(int id)
	{
		var image = await _postgresContext.ProfileImages.FindAsync(id);

		if (image == null)
			return NotFound();

		return File(image.ImageData, image.ContentType, image.FileName);
	}

	[HttpGet("postgresql/{id}/info")]
	public async Task<IActionResult> GetInfoFromPostgreSQL(int id)
	{
		var image = await _postgresContext.ProfileImages
			.Where(i => i.Id == id)
			.Select(i => new
			{
				i.Id,
				i.FileName,
				i.ContentType,
				i.FileSize,
				i.UploadedAt
			})
			.FirstOrDefaultAsync();

		if (image == null)
			return NotFound();

		return Ok(image);
	}

	[HttpDelete("postgresql/{id}")]
	public async Task<IActionResult> DeleteFromPostgreSQL(int id)
	{
		var image = await _postgresContext.ProfileImages.FindAsync(id);

		if (image == null)
			return NotFound();

		_postgresContext.ProfileImages.Remove(image);
		await _postgresContext.SaveChangesAsync();

		return NoContent();
	}

	

	[HttpGet("postgresql")]
	public async Task<IActionResult> ListFromPostgreSQL()
	{
		var images = await _postgresContext.ProfileImages
			.Select(i => new
			{
				i.Id,
				i.FileName,
				i.ContentType,
				i.FileSize,
				i.UploadedAt
			})
			.OrderByDescending(i => i.UploadedAt)
			.ToListAsync();

		return Ok(images);
	}



	/// <summary>
	/// Process profile image - resize and create thumbnail
	/// Supports HDR images and preserves quality
	/// </summary>
	private async Task<(byte[] resized, byte[] thumbnail)> ProcessProfileImage(IFormFile file)
	{
		using var inputStream = file.OpenReadStream();
		using var image = await Image.LoadAsync(inputStream);

		// Get original dimensions
		int originalWidth = image.Width;
		int originalHeight = image.Height;

		// Resize main image if needed (maintain aspect ratio)
		byte[] resizedBytes;
		if (originalWidth > MaxProfileWidth || originalHeight > MaxProfileHeight)
		{
			var resizeOptions = new ResizeOptions
			{
				Size = new Size(MaxProfileWidth, MaxProfileHeight),
				Mode = ResizeMode.Max, // Maintain aspect ratio
				Sampler = KnownResamplers.Lanczos3 // High quality resampling
			};

			image.Mutate(x => x.Resize(resizeOptions));
		}

		// Save main image as WebP with high quality
		using var resizedStream = new MemoryStream();
		await image.SaveAsWebpAsync(resizedStream, new WebpEncoder
		{
			Quality = 90, // High quality
			FileFormat = WebpFileFormatType.Lossy,
			Method = WebpEncodingMethod.BestQuality
		});
		resizedBytes = resizedStream.ToArray();

		// Create thumbnail
		var thumbnailOptions = new ResizeOptions
		{
			Size = new Size(ThumbnailSize, ThumbnailSize),
			Mode = ResizeMode.Crop, // Square crop for thumbnail
			Sampler = KnownResamplers.Lanczos3
		};

		image.Mutate(x => x.Resize(thumbnailOptions));

		using var thumbnailStream = new MemoryStream();
		await image.SaveAsWebpAsync(thumbnailStream, new WebpEncoder
		{
			Quality = 85,
			FileFormat = WebpFileFormatType.Lossy
		});
		byte[] thumbnailBytes = thumbnailStream.ToArray();

		return (resizedBytes, thumbnailBytes);
	}

	private string FormatFileSize(long bytes)
	{
		string[] sizes = { "B", "KB", "MB", "GB" };
		double len = bytes;
		int order = 0;

		while (len >= 1024 && order < sizes.Length - 1)
		{
			order++;
			len = len / 1024;
		}

		return $"{len:0.##} {sizes[order]}";
	}

		#endregion
}