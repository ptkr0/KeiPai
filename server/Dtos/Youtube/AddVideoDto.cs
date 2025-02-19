namespace Dtos.Youtube
{
	public class AddVideoDto
	{
		public ICollection<Models.Game> Games { get; set; } = new List<Models.Game>();
		public string InfluencerId { get; set; } = string.Empty;
		public string VideoId { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public ulong ViewCount { get; set; }
		public string Url { get; set; } = string.Empty;
		public string Thumbnail { get; set; } = string.Empty;
		public DateTimeOffset? UploadDate { get; set; }
	}
}
