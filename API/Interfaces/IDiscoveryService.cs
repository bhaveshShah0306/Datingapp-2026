using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Interfaces;

	public interface IDiscoveryService
	{
		Task<IEnumerable<Guid>> GetFeedAsync(ClaimsPrincipal user);
	}

