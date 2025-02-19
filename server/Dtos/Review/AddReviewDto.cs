namespace Dtos.Review
{
	public class AddReviewDto
	{
		public required string RevieweeId { get; set; }
		public int Rating { get; set; }
		public string? Comment { get; set; } = string.Empty;
		public bool IsAnonymous { get; set; }

	}
}
