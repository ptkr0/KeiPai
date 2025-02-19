using Dtos.Tag;

namespace Dtos.Game
{
	public class GameDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string CoverPhoto { get; set; } = string.Empty;
		public DateOnly ReleaseDate { get; set; }
		public ICollection<TagDto> Tags { get; set; } = new List<TagDto>(); // 1 game can have multiple tags
		public string? DeveloperId { get; set; } // foreign key to Developer table
		public string? DeveloperName { get; set; }
	}
}
