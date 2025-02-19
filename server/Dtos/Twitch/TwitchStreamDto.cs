namespace Dtos.Twitch
{
	public class TwitchStreamDto
	{
		public required string Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public int AverageViewers { get; set; }
		public int PeakViewers { get; set; }
		public string Thumbnail { get; set; } = string.Empty;
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
