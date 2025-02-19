namespace Dtos.Campaign
{
	public class CanUserRequestDto
	{
		public bool CanUserRequest { get; set; }
		public bool MeetsYoutubeSubscribersRequirement { get; set; }
		public bool MeetsTwitchFollowersRequirement { get; set; }
		public bool MeetsYoutubeViewsRequirement { get; set; }
		public bool MeetsTwitchViewsRequirement { get; set; }
		public bool MeetsThirdPartyWebsitesRequirement { get; set; }
		public bool RequiresManualApproval { get; set; }
	}
}
