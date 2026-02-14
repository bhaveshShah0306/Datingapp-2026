using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace API.SignalR
{
	[Authorize]
	public class PresenceHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			// ✅ Use UniqueName claim (which contains UserName from token)
			var username = Context.User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName);

			// Fallback to Name claim if UniqueName is not found
			username ??= Context.User?.FindFirstValue(ClaimTypes.Name);

			Console.WriteLine($"✅ User connected: {username ?? "Unknown"}");

			if (!string.IsNullOrEmpty(username))
			{
				await Clients.Others.SendAsync("UserOnline", username);
			}

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			// ✅ Use UniqueName claim (which contains UserName from token)
			var username = Context.User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName);

			// Fallback to Name claim if UniqueName is not found
			username ??= Context.User?.FindFirstValue(ClaimTypes.Name);

			Console.WriteLine($"❌ User disconnected: {username ?? "Unknown"}");

			if (!string.IsNullOrEmpty(username))
			{
				await Clients.Others.SendAsync("UserOffline", username);
			}

			await base.OnDisconnectedAsync(exception);
		}
	}
}
