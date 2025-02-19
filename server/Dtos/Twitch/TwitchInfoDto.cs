namespace Dtos.Twitch
{
	public class TwitchInfoDto
	{
		public string Username { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public int FollowerCount { get; set; }
		public int AverageViewers { get; set; }
		public bool IsAffiliateOrPartner { get; set; }
		public DateTime? LastCrawlDate { get; set; }
	}
}
