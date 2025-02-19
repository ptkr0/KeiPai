using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class OtherContent
	{
		[Key]
		public int ContentId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public string? Thumbnail { get; set; } = string.Empty;
		public DateTimeOffset PublishedAt { get; set; }
	}
}
