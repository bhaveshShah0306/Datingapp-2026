using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
		public static string GetUserTier(this ClaimsPrincipal user)
		{
			return user.FindFirst("tier")?.Value ?? "Free";
		}
		public static bool IsPremiumUser(this ClaimsPrincipal user)
		{
			return user.GetUserTier() == "Premium";
		}

		public static bool IsFreeOrAbove(this ClaimsPrincipal user)
		{
			var tier = user.GetUserTier();
			return tier == "Free" || tier == "Premium";
		}
	}
}