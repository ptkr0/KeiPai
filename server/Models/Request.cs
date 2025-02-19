using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Requests")]
	public class Request
	{
		public int Id { get; set; }
		public int CampaignId { get; set; }
		public Campaign? Campaign { get; set; }

		public required string InfluencerId { get; set; }
		public Influencer? Influencer { get; set; }

		public int PlatformId { get; set; }
		public Platform? Platform { get; set; }

		public DateTime RequestDate { get; set; } = DateTime.Now;
		public DateTime? ConsiderationDate { get; set; }

		public required string Media { get; set; } // 'youtube', 'twitch', 'other'
		public int Status { get; set; } = 0; // 0 = pending, 1 = accepted, 2 = rejected

		// only if key is granted to influencer
		public int? KeyId { get; set; }
		public Key? Key { get; set; }

		// only if influencer creates content for campaign
		public int? ContentId { get; set; }
		public Content? Content { get; set; }
	}
}
