using Dtos.OtherMedia;
using Dtos.Twitch;
using Dtos.Youtube;

namespace Dtos.Account
{
	public class InfluencerInfoDto
	{
		public ChannelInfoDetailedDto? Youtube { get; set; }
		public OtherMediaInfoDto? OtherMedia { get; set; }
		public TwitchInfoDto? Twitch { get; set; }
	}
}
