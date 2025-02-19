namespace Dtos.Campaign
{
	public class AssignedKeysDto
	{
		public int CampaignId { get; set; }
		public int? GameId { get; set; }
		public string GameName { get; set; } = string.Empty;
		public int PlatformId { get; set; }
		public string PlatformName { get; set; } = string.Empty;
		public int MaximumNumberOfKeys { get; set; }
		public int? NumberOfKeys { get; set; }
		public bool IsUnlimited { get; set; }
	}
}
