using Dtos.Game;

namespace Dtos.OtherMedia
{
	public class OtherContentDto
	{
		public int Id { get; set; }
		public required string Title { get; set; }
		public string Description { get; set; } = string.Empty;
		public required string Url { get; set; }
		public ICollection<BasicGameDto> Games { get; set; } = new List<BasicGameDto>();
		public string Thumbnail { get; set; } = string.Empty;
	}
}
