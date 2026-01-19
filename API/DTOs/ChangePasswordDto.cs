using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
	public class ChangePasswordDto
	{
		[Required]
		public string CurrentPassword { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 8)]
		public string NewPassword { get; set; }

		[Required]
		[Compare("NewPassword")]
		public string ConfirmPassword { get; set; }
	}
}
