using Dtos.Youtube;

namespace Interfaces
{
	public interface IYoutubeService
	{
		Task<ExchangedCodeForTokensDto?> ExchangeCodeForTokens(string code, string userId);
		Task<string?> GetAccessToken(string refreshToken, string userId);
		Task<ChannelInfoDto> GetChannelInfo(string accessToken);
		Task<List<YoutubeVideoDto>> GetAllVideos(string accessToken, string channelId, DateTimeOffset? publishedAfter);
	}
}
