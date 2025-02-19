using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class TwitchAccount
	{
		[Key]
		public required string UserId { get; set; }

		[Required]
		public required string TwitchChannelId { get; set; }

		public string ChannelName { get; set; } = string.Empty;
		public int FollowerCount { get; set; }
		public string? RefreshToken { get; set; } = string.Empty;
		public DateTime? LastCrawlDate { get; set; }
		public bool IsAffiliateOrPartner { get; set; }
	}
}
