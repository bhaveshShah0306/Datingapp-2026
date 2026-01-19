using API.Helpers;
using System.Security.Claims;

namespace API.Services;

	public class DiscoveryService :IDiscoveryService
	{
		private readonly ExposurePolicyService _exposure;
	private readonly RecommendationScorer _scorer;
	private readonly IUserRepository _repo;
		private readonly IStatsService _stats;

		public DiscoveryService(
			ExposurePolicyService exposure,
			RecommendationScorer scorer,
			IUserRepository repo,
			IStatsService stats)
		{
			_exposure = exposure;
			_scorer = scorer;
			_repo = repo;
			_stats = stats;
		}

		public async Task<IEnumerable<Guid>> GetFeedAsync(ClaimsPrincipal user)
		{

		if (user?.Identity?.IsAuthenticated != true || string.IsNullOrEmpty(user.Identity.Name))
		{
			throw new UnauthorizedAccessException("User must be authenticated");
		}
		var me = await _repo.GetMemberAsync(user.Identity!.Name!);

		var maleRatio = await _stats.GetMaleFemaleRatio(me.Country);
		var boost = _exposure.FromRatio(maleRatio);

		//var candidates = await _repo.GetMembersAsync(user);

		//var weighted = candidates.Select(c => new
		//{
		//	c.Id,
		//	Weight = _scorer.Score(
		//		me.Rating,
		//		c.Rating,
		//		c.Gender,
		//		boost)
		//});

		return Enumerable.Empty<Guid>();
		//return weighted
		//	.OrderByDescending(x => Random.Shared.NextDouble() * x.Weight)
		//	.Take(50)
		//	.Select(x => x.Id);
	}
	}

