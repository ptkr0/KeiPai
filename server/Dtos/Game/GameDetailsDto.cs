using Dtos.Tag;

namespace Dtos.Game
{
	public class GameDetailsDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string YoutubeTrailer { get; set; } = string.Empty;
		public string CoverPhoto { get; set; } = string.Empty;
		public DateOnly ReleaseDate { get; set; }
		public string YoutubeTag { get; set; } = string.Empty;
		public int? TwitchTagId { get; set; }
		public string TwitchTagName { get; set; } = string.Empty;
		public string MinimumCPU { get; set; } = string.Empty;
		public string MinimumRAM { get; set; } = string.Empty;
		public string MinimumGPU { get; set; } = string.Empty;
		public string MinimumStorage { get; set; } = string.Empty;
		public string MinimumOS { get; set; } = string.Empty;
		public string PressKit { get; set; } = string.Empty;

		public ICollection<TagDto> Tags { get; set; } = new List<TagDto>(); // 1 game can have multiple tags
		public ICollection<ScreenshotDto> Screenshots { get; set; } = new List<ScreenshotDto>(); // 1 game can have multiple screenshots

		public string? DeveloperId { get; set; } // foreign key to Developer table
		public string? DeveloperName { get; set; } //developer name
	}
}
