using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; }
       [Required] public string Email { get; set; }
		[Required] public string KnownAs { get; set; }
         public string Gender { get; set; }
        
		public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8,ErrorMessage = "Password must be at least 8 characters long")]
		public string Password { get; set; }
    }
}