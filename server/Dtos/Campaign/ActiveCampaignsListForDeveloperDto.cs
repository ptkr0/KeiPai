namespace Dtos.Campaign
{
	public class ActiveCampaignsListForDeveloperDto
	{
		public int Id { get; set; }
		public int GameId { get; set; }
		public string? GameName { get; set; }
		public ICollection<RequestsSentAndKeysLeftDto> Keys { get; set; } = new List<RequestsSentAndKeysLeftDto>();
	}
}
