using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Games")]
	public class Game
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(50)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(5000)]
		public string Description { get; set; } = string.Empty;

		[MaxLength(50)]
		public string? YoutubeTrailer { get; set; } = string.Empty;
		public string? CoverPhoto { get; set; }
		public DateOnly ReleaseDate { get; set; }

		[MaxLength(50)]
		public string? YoutubeTag { get; set; } = string.Empty;

		[MaxLength(50)]
		public int? TwitchTagId { get; set; }

		[MaxLength(50)]
		public string? TwitchTagName { get; set; } = string.Empty;

		[MaxLength(50)]
		public string? MinimumCPU { get; set; } = string.Empty;

		[MaxLength(50)]
		public string? MinimumRAM { get; set; } = string.Empty;

		[MaxLength(50)]
		public string? MinimumGPU { get; set; } = string.Empty;

		[MaxLength(50)]
		public string? MinimumStorage { get; set; } = string.Empty;

		[MaxLength(50)]
		public string? MinimumOS { get; set; } = string.Empty;

		[MaxLength(50)]
		public string? PressKit { get; set; } = string.Empty;

		public ICollection<Tag> Tags { get; set; } = new List<Tag>(); // 1 game can have multiple tags
		public ICollection<GameScreenshot> Screenshots { get; set; } = new List<GameScreenshot>(); // 1 game can have multiple screenshots
		public ICollection<Key> Keys { get; set; } = new List<Key>(); // 1 game can have multiple keys 
		public ICollection<Content> Contents { get; set; } = new List<Content>(); // 1 game can have multiple contents meade about it (videos, streams, etc)
		public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>(); // 1 game can have multiple campaigns
		public string? DeveloperId { get; set; } // foreign key to Developer table
		public Developer? Developer { get; set; }
	}
}
