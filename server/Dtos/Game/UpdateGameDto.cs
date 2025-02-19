using System.ComponentModel.DataAnnotations;

namespace Dtos.Game
{
	public class UpdateGameDto
	{
		[MaxLength(50)]
		public string? Name { get; set; }

		[MaxLength(5000)]
		public string? Description { get; set; }

		[MaxLength(50)]
		public string? YoutubeTrailer { get; set; }

		public DateOnly? ReleaseDate { get; set; }

		[MaxLength(50)]
		public string? YoutubeTag { get; set; }

		public int? TwitchTagId { get; set; }

		[MaxLength(50)]
		public string? TwitchTagName { get; set; }

		[MaxLength(50)]
		public string? MinimumCPU { get; set; }

		[MaxLength(50)]
		public string? MinimumRAM { get; set; }

		[MaxLength(50)]
		public string? MinimumGPU { get; set; }

		[MaxLength(50)]
		public string? MinimumStorage { get; set; }

		[MaxLength(50)]
		public string? MinimumOS { get; set; }

		[MaxLength(50)]
		public string? PressKit { get; set; }

		public List<int>? Tags { get; set; }

	}
}
