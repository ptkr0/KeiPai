using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Campaigns")]
	public class Campaign
	{
		[Key]
		public int Id { get; set; }

		public int? GameId { get; set; } // foreign key to Game table
		public Game? Game { get; set; }

		[Required]
		public DateTime CreationDate { get; set; } = DateTime.Now;

		[Required]
		public DateTime StartDate { get; set; } = DateTime.Now;
		public DateTime? EndDate { get; set; }

		[Required]
		[MaxLength(1000)]
		public string? Description { get; set; } = string.Empty;

		public int? MinimumYoutubeSubscribers { get; set; } = null;
		public int? MinimumTwitchFollowers { get; set; } = null;
		public int? MinimumTwitchAvgViewers { get; set; } = null;
		public int? MinimumYoutubeAvgViews { get; set; } = null;
		public bool AutoCodeDistribution { get; set; } = false;
		public DateTime? EmbargoDate { get; set; }
		public int AreThirdPartyWebsitesAllowed { get; set; } = 0; // 0 = no, 1 = yes, 2 = yes but with restrictions
		public ICollection<CampaignKey> Keys { get; set; } = new List<CampaignKey>(); // 1 campaign can have multiple keys to multiple platforms
		public ICollection<Request> Requests { get; set; } = new List<Request>(); // 1 campaign can have multiple requests
		public bool IsClosed { get; set; } = false;
	}
}
