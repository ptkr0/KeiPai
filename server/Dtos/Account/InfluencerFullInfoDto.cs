using Dtos.Review;

namespace Dtos.Account
{
	public class InfluencerFullInfoDto
	{
		public InfluencerDto Influencer { get; set; } = new InfluencerDto();
		public InfluencerInfoDto Media { get; set; } = new InfluencerInfoDto();
		public UserRatingDto Rating { get; set; } = new UserRatingDto();
		public int RequestsSent { get; set; }
		public int RequestsDone { get; set; }
	}
}
