namespace Dtos.Campaign
{
	public class UpdatedCampaignDto
	{
		public int Id { get; set; }
		public int? GameId { get; set; }
		public string GameName { get; set; }
		public string Description { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public int AreThirdPartyWebsitesAllowed { get; set; }
		public int? MinimumTwitchAvgViewers { get; set; }
		public int? MinimumTwitchFollowers { get; set; }
		public int? MinimumYoutubeAvgViews { get; set; }
		public int? MinimumYoutubeSubscribers { get; set; }
		public bool AutoCodeDistribution { get; set; }
		public DateTime? EmbargoDate { get; set; }
	}
}
