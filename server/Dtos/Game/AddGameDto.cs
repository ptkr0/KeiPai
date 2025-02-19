using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Dtos.Game
{
	public class AddGameDto
	{
		[Required]
		[MaxLength(50)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(5000)]
		public string Description { get; set; } = string.Empty;

		[MaxLength(50)]
		public string? YoutubeTrailer { get; set; } = string.Empty;

		[Required]
		public DateOnly ReleaseDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

		[MaxLength(50)]
		[SwaggerSchema(Description = "tag to influencer to put in tags section on youtube")]
		public string? YoutubeTag { get; set; } = string.Empty;

		public int? TwitchTagId { get; set; }

		[MaxLength(50)]
		[SwaggerSchema(Description = "game name to influencer to select on twitch. will need to be taken from IGDB")]
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
		[SwaggerSchema(Description = "URL to press kit, might be changed to .zip file in the future")]
		public string? PressKit { get; set; } = string.Empty;

		[SwaggerSchema(Description = "list of tag ids. max number of tags is 5; more will result in error")]
		public List<int>? Tags { get; set; } = new List<int>();

		[SwaggerSchema(Description = "file needs to be in .jpg, .jpeg, .png or .webp. max file size can be 1mb")]
		public IFormFile? CoverPhoto { get; set; }
		//public List<IFormFile>? Screenshots { get; set; } = new List<IFormFile>();
	}
}
