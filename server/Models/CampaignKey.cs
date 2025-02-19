using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("CampaignKeys")]
	public class CampaignKey
	{
		[Key]
		public int CampaignId { get; set; }
		public Campaign? Campaign { get; set; }

		public int PlatformId { get; set; }
		public Platform? Platform { get; set; }

		public int MaximumNumberOfKeys { get; set; }
		public bool IsUnlimited { get; set; } = false;
	}
}
