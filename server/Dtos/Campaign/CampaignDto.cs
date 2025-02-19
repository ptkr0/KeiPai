using Dtos.Game;

namespace Dtos.Campaign
{
	public class CampaignDto
	{
		public int Id { get; set; }
		public GameDto Game { get; set; } = new GameDto();
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public bool IsClosed { get; set; }
		public bool? DidJoin { get; set; }
		public ICollection<KeysLeftForCampaignDto> KeysLeftForCampaign { get; set; } = new List<KeysLeftForCampaignDto>();
	}
}
