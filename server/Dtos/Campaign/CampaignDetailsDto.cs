using Dtos.Game;

namespace Dtos.Campaign
{
	public class CampaignDetailsDto
	{
		public int Id { get; set; }
		public GameDetailsDto Game { get; set; } = new GameDetailsDto();
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Description { get; set; } = string.Empty;
		public int? MinimumYoutubeSubscribers { get; set; }
		public int? MinimumTwitchFollowers { get; set; }
		public int? MinimumTwitchAvgViewers { get; set; }
		public int? MinimumYoutubeAvgViews { get; set; }
		public bool AutoCodeDistribution { get; set; }
		public DateTime? EmbargoDate { get; set; }
		public int AreThirdPartyWebsitesAllowed { get; set; }
		public bool IsClosed { get; set; }
		public ICollection<KeysLeftForCampaignDto> KeysLeftForCampaigns { get; set; } = new List<KeysLeftForCampaignDto>();
	}
}
