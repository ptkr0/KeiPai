using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class YoutubeAccount
	{
		[Key]
		public required string UserId { get; set; }

		[Required]
		public required string YoutubeChannelId { get; set; }

		public string ChannelName { get; set; } = string.Empty;
		public ulong SubscriberCount { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? LastCrawlDate { get; set; }
	}
}
