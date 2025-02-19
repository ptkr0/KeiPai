namespace Dtos.Request
{
	public class InfluencerRequestDto
	{
		public int Id { get; set; }
		public int CampaignId { get; set; }
		public int GameId { get; set; }
		public string GameName { get; set; } = string.Empty;
		public string GameCover { get; set; } = string.Empty;
		public int Platform { get; set; }
		public string Media { get; set; } = string.Empty;
		public DateTime RequestDate { get; set; }
		public DateTime? ConsiderationDate { get; set; }
		public string? Key { get; set; }
		public int Status { get; set; }
		public int? Content { get; set; }
	}
}
