namespace Dtos.Campaign
{
	public class CampaignStatsDto
	{
		public CampaignDto Campaign { get; set; } = new CampaignDto();
		public ICollection<RequestsSentAndKeysLeftDto> Keys { get; set; } = new List<RequestsSentAndKeysLeftDto>();
	}
}
