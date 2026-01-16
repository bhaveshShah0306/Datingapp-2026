namespace API.DTOs;

	public class WorldBankDataDto
	{
		public IndicatorInfo indicator { get; set; }
		public CountryData country { get; set; }
		public string countryiso3code { get; set; }
		public string date { get; set; }
		public double? value { get; set; }
		public string unit { get; set; }
		public string obs_status { get; set; }
		public int decimalPlaces { get; set; } = 0;
}

public class IndicatorInfo
{
}

public class CountryData
{
}