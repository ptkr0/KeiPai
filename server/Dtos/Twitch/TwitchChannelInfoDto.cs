namespace Dtos.Twitch
{
	public class TwitchChannelInfoDto
	{
		public string ChannelId { get; set; } = string.Empty;
		public string ChannelName { get; set; } = string.Empty;
		public int FollowerCount { get; set; }
		public bool IsAffiliateOrPartner { get; set; } = false;
	}
}
