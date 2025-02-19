using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Contents")]
	public class Content
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public required string Type { get; set; }

		public ICollection<Game>? Games { get; set; } = new List<Game>();

		public Influencer? Influencer { get; set; }

		[ForeignKey("Influencer")]
		public string? UserId { get; set; }

		public OtherContent? OtherContent { get; set; }
		public YoutubeVideo? YoutubeVideo { get; set; }
		public TwitchStream? TwitchStream { get; set; }
	}
}
