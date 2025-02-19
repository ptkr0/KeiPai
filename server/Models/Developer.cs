using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Developers")]
	public class Developer
	{
		[Key]
		public required string UserId { get; set; }

		public User? User { get; set; }

		public string? WebsiteUrl { get; set; } = string.Empty;

		public ICollection<Game> Games { get; set; } = new List<Game>(); // 1 developer can have multiple games
		public ICollection<Review> Reviews { get; set; } = new List<Review>(); // reviews left by the user
	}
}
