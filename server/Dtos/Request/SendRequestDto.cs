namespace Dtos.Request
{
	public class SendRequestDto
	{
		public int CampaignId { get; set; }
		public int PlatformId { get; set; }
		public required string ContentPlatformType { get; set; }
	}
}
