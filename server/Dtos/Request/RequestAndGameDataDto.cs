namespace Dtos.Request
{
	public class RequestAndGameDataDto
	{
		public required Models.Request Request { get; set; }
		public int GameId { get; set; }
		public string? YoutubeTag { get; set; }
		public int? TwitchTagId { get; set; }
		public string? TwitchTagName { get; set; }
	}
}
