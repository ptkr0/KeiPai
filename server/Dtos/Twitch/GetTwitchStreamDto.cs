using Dtos.Game;

namespace Dtos.Twitch
{
	public class GetTwitchStreamDto
	{
		public int Id { get; set; }
		public ICollection<BasicGameDto> Games { get; set; } = new List<BasicGameDto>();
		public required TwitchStreamDto Stream { get; set; }
	}
}
