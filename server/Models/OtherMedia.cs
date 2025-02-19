using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class OtherMedia
	{
		[Key]
		public required string UserId { get; set; }

		public required string Name { get; set; }
		public required string Url { get; set; }
		public string Role { get; set; } = string.Empty;
		public string Medium { get; set; } = string.Empty;
		public string SampleContent { get; set; } = string.Empty;
		public required bool IsVerified { get; set; } = false;
	}
}
