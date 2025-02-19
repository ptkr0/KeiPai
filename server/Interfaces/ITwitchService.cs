using Dtos.Twitch;
using Dtos.Youtube;
using Models;

namespace Interfaces
{
	public interface ITwitchService
	{
		Task<ExchangedCodeForTokensDto?> ExchangeCodeForTokens(string code);
		Task<ExchangedCodeForTokensDto?> GetAccessToken(string refreshToken);
		Task<TwitchChannelInfoDto?> GetChannelInfo(string accessToken);
		Task<TwitchStreamSnapshot?> GetStreamInfo(string accessToken, string channelId);
		Task<AdditionalStreamInfoDto?> GetAdditionalStreamInfo(string accessToken, string channelId);
		Task<bool> SubscribeToWebhooks(string broadcasterId);
		Task<string?> GetAppAccessToken();
		Task<bool> UnsubscribeFromWebhooks(string broadcasterId);
	}
}
