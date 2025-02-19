using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Reviews")]
	public class Review
	{
		[Key]
		public int Id { get; set; }

		public required string ReviewerId { get; set; }
		public Developer? Reviewer { get; set; }

		public required string RevieweeId { get; set; }
		public Influencer? Reviewee { get; set; }

		public int Rating { get; set; } // 1-5
		public string? Comment { get; set; } = string.Empty;
		public DateTime ReviewDate { get; set; } = DateTime.Now;
		public bool IsAnonymous { get; set; } = false;
	}
}
