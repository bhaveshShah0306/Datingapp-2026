using System.Collections.Concurrent;

namespace API.SignalR
{
	public class PresenceTracker
	{
		private static readonly ConcurrentDictionary<string,Lazy<ConcurrentDictionary<string,byte>>> OnlineUsers = new();

		public Task UserConnected(string username, string connectionId)
		{
			var lazyConnections = OnlineUsers.GetOrAdd(username, _ => new Lazy<ConcurrentDictionary<string, byte>>(() => new ConcurrentDictionary<string, byte>()));
			var connections = lazyConnections.Value;

			connections.TryAdd(connectionId, 0);
			return Task.CompletedTask;
		}

		public Task UserDisconnected(string username, string connectionId)
		{
			if (OnlineUsers.TryGetValue(username, out var lazyConnections))
			{
				lazyConnections.Value.TryRemove(connectionId, out _);
				if (lazyConnections.Value.IsEmpty) 
				{
					OnlineUsers.TryRemove(username, out _);
				}
			}
			return Task.CompletedTask;
		}

		public Task<string[]> GetOnlineUsers()
		{			
			return Task.FromResult(OnlineUsers.Keys.OrderBy(k => k).ToArray());
		}
	}
}