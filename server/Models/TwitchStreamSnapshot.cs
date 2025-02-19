using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class TwitchStreamSnapshot
	{
		[Key]
		public int Id { get; set; }

		public string StreamId { get; set; } = string.Empty;

		public string Title { get; set; } = string.Empty;
		public string? IGDBName { get; set; } = string.Empty;
		public int ViewerCount { get; set; }
		public DateTime SnapshotDate { get; set; }
	}
}
