namespace API.Services
{
	public class ExposurePolicyService
	{
		public ExposureBoost FromRatio(double maleRatio)
		{
			if (maleRatio > 1.2)      // more men than women
				return new ExposureBoost(1.0, 1.25);

			if (maleRatio < 0.8)      // more women than men
				return new ExposureBoost(1.25, 1.0);

			return new ExposureBoost(1.0, 1.0);
		}
	}
}
