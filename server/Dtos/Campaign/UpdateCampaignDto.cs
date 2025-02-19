using System.ComponentModel.DataAnnotations;

namespace Dtos.Campaign
{
	public class UpdateCampaignDto
	{
		[Required]
		public string Description { get; set; }

		[Required]
		public DateTime StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		[Required]
		public int AreThirdPartyWebsitesAllowed { get; set; }

		public int? MinimumTwitchAvgViewers { get; set; }
		public int? MinimumTwitchFollowers { get; set; }
		public int? MinimumYoutubeAvgViews { get; set; }
		public int? MinimumYoutubeSubscribers { get; set; }
		public bool AutoCodeDistribution { get; set; }
		public DateTime? EmbargoDate { get; set; }
	}

}
