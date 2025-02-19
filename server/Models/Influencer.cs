using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Influencers")]
	public class Influencer
	{
		[Key]
		public required string UserId { get; set; }
		public User? User { get; set; }

		public string? Language { get; set; } = string.Empty;
		public string? SteamHandle { get; set; } = string.Empty;

		public YoutubeAccount? YoutubeAccount { get; set; }
		public TwitchAccount? TwitchAccount { get; set; }
		public OtherMedia? OtherMedia { get; set; }
		public ICollection<Content> Contents { get; set; } = new List<Content>(); // 1 influencer can have multiple contents
		public ICollection<Request> Requests { get; set; } = new List<Request>(); // 1 influencer can have multiple requests
		public ICollection<Review> Reviews { get; set; } = new List<Review>(); // reviews left by other users
	}
}
