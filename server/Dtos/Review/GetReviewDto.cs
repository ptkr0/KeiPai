namespace Dtos.Review
{
	public class GetReviewDto
	{
		public int Id { get; set; }
		public string? ReviewerId { get; set; }
		public string? ReviewerName { get; set; }
		public int Rating { get; set; }
		public string? Comment { get; set; }
		public DateTime ReviewDate { get; set; }
		public bool IsAnonymous { get; set; }
	}
}
