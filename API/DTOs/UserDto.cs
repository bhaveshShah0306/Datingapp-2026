namespace API.DTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }
        public string Tier { get; set; } = "Free";
		//public bool IsSubscriptionActive { get; set; }
		//public bool IsTwoFactorEnabled { get; set; }
        //public bool RequiresTwoFactorAuthentication { get; set; }
	}
}