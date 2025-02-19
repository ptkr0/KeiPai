using Dtos.Game;

namespace Dtos.Youtube
{
	public class GetYoutubeVideoDto
	{
		public int Id { get; set; }
		public ICollection<BasicGameDto> Games { get; set; } = new List<BasicGameDto>();
		public required YoutubeVideoDto Video { get; set; }
	}
}
