namespace Dtos.Youtube
{
	public class ChannelInfoDetailedDto
	{
		public string Username { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public ulong SubscriberCount { get; set; }
		public ulong ViewCount { get; set; }
		public ulong AverageViewCount { get; set; }
		public DateTime? LastCrawlDate { get; set; }
	}
}
