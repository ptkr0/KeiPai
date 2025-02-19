using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class TwitchStream
	{
		[Key]
		public int ContentId { get; set; }

		[Required]
		public string StreamId { get; set; } = string.Empty;

		public string? Title { get; set; } = string.Empty;
		public int? AverageViewers { get; set; }
		public int? PeakViewers { get; set; }
		public string? Url { get; set; } = string.Empty;
		public string? Thumbnail { get; set; } = string.Empty;
		public DateTime StartDate { get; set; } = DateTime.Now;
		public DateTime? EndDate { get; set; }
	}
}
