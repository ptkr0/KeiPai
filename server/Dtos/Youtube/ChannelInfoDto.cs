namespace Dtos.Youtube
{
	public class ChannelInfoDto
	{
		public string ChannelId { get; set; } = string.Empty;
		public string ChannelName { get; set; } = string.Empty;
		public ulong SubscriberCount { get; set; }
	}
}
