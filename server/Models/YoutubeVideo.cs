using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class YoutubeVideo
	{
		[Key]
		public int ContentId { get; set; }

		[Required]
		public required string VideoId { get; set; } = string.Empty;

		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public ulong ViewCount { get; set; }
		public string Url { get; set; } = string.Empty;
		public string Thumbnail { get; set; } = string.Empty;
		public DateTimeOffset? UploadDate { get; set; }
	}
}
