namespace Dtos.Campaign
{
	public class AddedCampaignDto
	{
		public int Id { get; set; }
		public int? GameId { get; set; }
		public string GameName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public int? MinimumYoutubeSubscribers { get; set; }
		public int? MinimumTwitchFollowers { get; set; }
		public int? MinimumTwitchAvgViewers { get; set; }
		public int? MinimumYoutubeAvgViews { get; set; }
		public bool AutoCodeDistribution { get; set; }
		public DateTime? EmbargoDate { get; set; }
		public int AreThirdPartyWebsitesAllowed { get; set; }
	}
}
