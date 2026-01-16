using API.Services;

namespace API.Helpers
{
	public class RecommendationScorer
	{
		public double Score(
			double viewerRating,
			double candidateRating,
			string candidateGender,
			ExposureBoost boost)
		{
			var ratingWeight =
				Math.Exp(-Math.Abs(candidateRating - viewerRating) / 200.0);

			var exposureBoost = candidateGender switch
			{
				"M" => boost.Male,
				"F" => boost.Female,
				_ => 1.0
			};

			return ratingWeight * exposureBoost;
		}
	}
}
