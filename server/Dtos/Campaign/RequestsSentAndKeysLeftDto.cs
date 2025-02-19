namespace Dtos.Campaign
{
	public class RequestsSentAndKeysLeftDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int AcceptedRequests { get; set; }
		public int KeysLeft { get; set; }
		public int KeysForCampaign { get; set; }
	}
}
