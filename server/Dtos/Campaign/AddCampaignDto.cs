using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Dtos.Campaign
{
	public class AddCampaignDto
	{
		public DateTime StartDate { get; set; } = DateTime.Now;

		[SwaggerSchema(Description = "if not null it has to be after StartDate.")]
		public DateTime? EndDate { get; set; }

		[Required]
		public int GameId { get; set; }

		[Required]
		[MaxLength(1000)]
		public string? Description { get; set; } = string.Empty;

		[SwaggerSchema(Description = "minimum number of subscribers needed to apply for this campaign. if null then disabled, if 0 then no restrictions")]
		public int? MinimumYoutubeSubscribers { get; set; }

		[SwaggerSchema(Description = "minimum number of followers needed to apply for this campaign. if null then disabled, if 0 then no restrictions")]
		public int? MinimumTwitchFollowers { get; set; }

		[SwaggerSchema(Description = "minimum number of avg viewers needed to apply for this campaign. if null then disabled, if 0 then no restrictions")]
		public int? MinimumTwitchAvgViewers { get; set; }

		[SwaggerSchema(Description = "minimum number of avg viewers needed to apply for this campaign. if null then disabled, if 0 then no restrictions")]
		public int? MinimumYoutubeAvgViews { get; set; }

		[SwaggerSchema(Description = "if true then the keys will be automatically distributed to the influencers")]
		public bool AutoCodeDistribution { get; set; }
		public DateTime? EmbargoDate { get; set; }

		[SwaggerSchema(Description = "0 = no, 1 = yes, 2 = yes but with restrictions")]
		public int AreThirdPartyWebsitesAllowed { get; set; } = 0; // 0 = no, 1 = yes, 2 = yes but with restrictions

		public ICollection<AssignKeysDto> Keys { get; set; } = new List<AssignKeysDto>();
	}
}
