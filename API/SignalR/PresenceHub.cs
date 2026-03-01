using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace API.SignalR
{
	[Authorize]
	public class PresenceHub(PresenceTracker presenceTracker) : Hub
	{
		public override async Task OnConnectedAsync()
		{

			await presenceTracker.UserConnected(GetUserId(),Context.ConnectionId);
			// ✅ Use UniqueName claim (which contains UserName from token)
			
				await Clients.Others.SendAsync("UserOnline", GetUserId());	
			
			var currentUsers = await presenceTracker.GetOnlineUsers();
			await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
			
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			// ✅ Use UniqueName claim (which contains UserName from token)			
				await presenceTracker.UserDisconnected(GetUserId(), Context.ConnectionId);
				await Clients.Others.SendAsync("UserOffline", GetUserId());
			var currentUsers = await presenceTracker.GetOnlineUsers();
			await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

			await base.OnDisconnectedAsync(exception);
		}

		private string GetUserId()
		{
			return Context.User?.GetUserId().ToString()?? throw new HubException("Cannot get user id");			
		}
	}
}
