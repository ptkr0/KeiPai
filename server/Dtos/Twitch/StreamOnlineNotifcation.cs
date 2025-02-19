namespace Dtos.Twitch
{
	public class StreamOnlineNotifcation
	{
		public required string id { get; set; }
		public required string broadcaster_user_id { get; set; }
		public required string broadcaster_user_login { get; set; }
		public required string broadcaster_user_name { get; set; }
		public required string type { get; set; }
		public required string started_at { get; set; }
	}
}
