namespace Dtos.Twitch
{
	public class StreamOfflineNotification
	{
		public required string broadcaster_user_id { get; set; }
		public required string broadcaster_user_login { get; set; }
		public required string broadcaster_user_name { get; set; }
	}
}
