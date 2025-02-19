using Dtos.Key;

namespace Dtos.Game
{
	public class GameForCampaignDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string CoverPhoto { get; set; } = string.Empty;
		public string YoutubeTag { get; set; } = string.Empty;
		public int? TwitchTagId { get; set; }
		public string TwitchTagName { get; set; } = string.Empty;
		public DateOnly ReleaseDate { get; set; }
		public ICollection<PlatformAndCountDto>? KeysPerPlatform { get; set; } // 1 game can have multiple keys per platform
	}
}
