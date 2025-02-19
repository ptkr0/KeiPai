namespace Dtos.Campaign
{
	public class CanRequestDto
	{
		public bool CanRequest { get; set; }
		public int ReasonCode { get; set; }
		public string? ReasonMessage { get; set; } = string.Empty;
	}
}
