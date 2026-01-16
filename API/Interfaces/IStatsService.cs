namespace API.Interfaces;

	public interface IStatsService
	{
	Task<double> GetMaleFemaleRatio(string country);	
	}

