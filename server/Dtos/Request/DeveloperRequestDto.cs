using Dtos.Review;

namespace Dtos.Request
{
	public class DeveloperRequestDto
	{
		public int Id { get; set; }
		public int CampaignId { get; set; }
		public int GameId { get; set; }
		public string GameName { get; set; } = string.Empty;
		public string InfluencerId { get; set; } = string.Empty;
		public string InfluencerName { get; set; } = string.Empty;
		public UserRatingDto InfluencerRating { get; set; } = new UserRatingDto();
		public int Platform { get; set; }
		public string Media { get; set; } = string.Empty;
		public int Status { get; set; }
		public int? Content { get; set; }
		public string Language { get; set; } = string.Empty;
		public DateTime RequestDate { get; set; }
		public DateTime? ConsiderationDate { get; set; }
	}
}
