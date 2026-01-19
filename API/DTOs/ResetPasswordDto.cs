using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
	public class ResetPasswordDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Token { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
		public string NewPassword { get; set; }

		[Required]
		[Compare("NewPassword", ErrorMessage = "Passwords do not match")]
		public string ConfirmPassword { get; set; }
	}
}
