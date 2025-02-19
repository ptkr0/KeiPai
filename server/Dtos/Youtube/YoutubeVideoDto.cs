namespace Dtos.Youtube
{
	public class YoutubeVideoDto
	{
		public required string Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public ulong ViewCount { get; set; }
		public string Thumbnail { get; set; } = string.Empty;
		public DateTimeOffset? UploadDate { get; set; }
		public IList<string> Tags { get; set; } = new List<string>();
	}
}
