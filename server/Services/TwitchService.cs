using Dtos.Twitch;
using Dtos.Youtube;
using Interfaces;
using Models;
using System.Text.Json;
using TwitchLib.Api;

namespace Services
{
	public class TwitchService : ITwitchService
	{
		private readonly ILogger<ITwitchService> _logger;
		private readonly IConfiguration _config;
		private readonly TwitchAPI _twitch;

		private readonly string _clientId;
		private readonly string _secret;


		public TwitchService(IConfiguration configuration, ILogger<ITwitchService> logger)
		{
			_config = configuration;
			_logger = logger;
			_twitch = new TwitchAPI();
			_clientId = _config["Authentication:Twitch:ClientId"] ?? throw new InvalidOperationException("Twitch ClientId is required.");
			_twitch.Settings.ClientId = _clientId;
			_secret = _config["Authentication:Twitch:ClientSecret"] ?? throw new InvalidOperationException("Twitch Client Secret is required.");
			_twitch.Settings.Secret = _secret;
		}


		public async Task<ExchangedCodeForTokensDto?> ExchangeCodeForTokens(string code)
		{
			try
			{
				var redirectUri = _config["Client:Http"] + "/twitch-success";

				var response = await _twitch.Auth.GetAccessTokenFromCodeAsync(code, _secret, redirectUri, _clientId);
				return new ExchangedCodeForTokensDto
				{
					AccessToken = response.AccessToken,
					RefreshToken = response.RefreshToken
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "BadRequestException occurred while exchanging code for tokens");
				return null;
			}
		}

		public async Task<ExchangedCodeForTokensDto?> GetAccessToken(string refreshToken)
		{
			try
			{
				var response = await _twitch.Auth.RefreshAuthTokenAsync(refreshToken, _secret, _clientId);
				return new ExchangedCodeForTokensDto
				{
					AccessToken = response.AccessToken,
					RefreshToken = response.RefreshToken
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "BadRequestException occurred while refreshing access token");
				return null;
			}
		}

		public async Task<TwitchChannelInfoDto?> GetChannelInfo(string accessToken)
		{
			_twitch.Settings.AccessToken = accessToken;

			var usersResponse = await _twitch.Helix.Users.GetUsersAsync();
			var user = usersResponse.Users.FirstOrDefault();

			if (user == null) return null;

			var channelName = user!.DisplayName;
			var channelId = user.Id;
			var isAffiliate = user.BroadcasterType == "affiliate" || user.BroadcasterType == "partner";

			var followers = await _twitch.Helix.Channels.GetChannelFollowersAsync(channelId);

			return new TwitchChannelInfoDto
			{
				ChannelId = channelId,
				ChannelName = channelName,
				FollowerCount = followers.Total,
				IsAffiliateOrPartner = isAffiliate
			};
		}

		public async Task<TwitchStreamSnapshot?> GetStreamInfo(string accessToken, string channelId)
		{
			try
			{
				_twitch.Settings.AccessToken = accessToken;

				var streamResponse = await _twitch.Helix.Streams.GetStreamsAsync(userIds: new List<string> { channelId });
				if (streamResponse == null) return null;

				var stream = streamResponse.Streams.FirstOrDefault();
				if (stream == null) return null;

				var streamSnapshot = new TwitchStreamSnapshot
				{
					StreamId = stream.Id,
					ViewerCount = stream.ViewerCount,
					Title = stream.Title,
					IGDBName = stream.GameName,
					SnapshotDate = DateTime.Now,
				};

				return streamSnapshot;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get stream info");
				return null;
			}
		}

		public async Task<bool> SubscribeToWebhooks(string broadcasterId)
		{
			string callbackUrl = _config["Ngrok"] ?? throw new InvalidOperationException("Ngrok Url required.");
			string webhookSecret = _config["Authentication:Twitch:WebhookSecret"] ?? throw new InvalidOperationException("Twitch Webhook Secret is required.");

			try
			{
				var token = await GetAppAccessToken();
				_twitch.Settings.AccessToken = token;

				await _twitch.Helix.EventSub.CreateEventSubSubscriptionAsync(
					type: "stream.online",
					version: "1",
					condition: new Dictionary<string, string> { { "broadcaster_user_id", broadcasterId } },
					method: TwitchLib.Api.Core.Enums.EventSubTransportMethod.Webhook,
					webhookCallback: callbackUrl + "/api/twitch/eventsub",
					webhookSecret: webhookSecret,
					clientId: _clientId,
					accessToken: token
					);

				await _twitch.Helix.EventSub.CreateEventSubSubscriptionAsync(
					type: "stream.offline",
					version: "1",
					condition: new Dictionary<string, string> { { "broadcaster_user_id", broadcasterId } },
					method: TwitchLib.Api.Core.Enums.EventSubTransportMethod.Webhook,
					webhookCallback: callbackUrl + "/api/twitch/eventsub",
					webhookSecret: webhookSecret,
					clientId: _clientId,
					accessToken: token
					);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to subscribe to webhooks");
				return false;
			}
		}


		/// <summary>
		/// gets app access token for twitch api
		/// thwitchlib doesn't return app access token, idk why
		/// </summary>
		/// <returns></returns>
		public async Task<string?> GetAppAccessToken()
		{
			string TokenUrl = "https://id.twitch.tv/oauth2/token";

			using (var httpClient = new HttpClient())
			{
				var postData = new FormUrlEncodedContent(new[]
				{
				new KeyValuePair<string, string>("client_id", _clientId),
				new KeyValuePair<string, string>("client_secret", _secret),
				new KeyValuePair<string, string>("grant_type", "client_credentials")
			});

				try
				{
					var response = await httpClient.PostAsync(TokenUrl, postData);
					response.EnsureSuccessStatusCode();

					var responseBody = await response.Content.ReadAsStringAsync();

					var jsonResponse = JsonDocument.Parse(responseBody);
					var accessToken = jsonResponse.RootElement.GetProperty("access_token").GetString();

					return accessToken;
				}
				catch (HttpRequestException ex)
				{
					_logger.LogError(ex, "Failed to get app access token");
					return null;
				}
			}
		}

		public async Task<bool> UnsubscribeFromWebhooks(string broadcasterId)
		{
			try
			{
				if (_twitch.Settings.AccessToken == null)
				{
					var token = await GetAppAccessToken();
					_twitch.Settings.AccessToken = token;
				}

				var webhooks = await _twitch.Helix.EventSub.GetEventSubSubscriptionsAsync(userId: broadcasterId);

				foreach (var webhook in webhooks.Subscriptions)
				{
					await _twitch.Helix.EventSub.DeleteEventSubSubscriptionAsync(webhook.Id);
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to unsubscribe from webhooks");
				return false;
			}
		}

		/// <summary>
		/// uses twitch api to get additional stream info (url, thumbnail) from a saved vod (the latest one)
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="channelId"></param>
		/// <returns></returns>
		public async Task<AdditionalStreamInfoDto?> GetAdditionalStreamInfo(string accessToken, string channelId)
		{
			_twitch.Settings.AccessToken = accessToken;

			try
			{
				var info = await _twitch.Helix.Videos.GetVideosAsync(userId: channelId,
																	 first: 1,
																	 type: TwitchLib.Api.Core.Enums.VideoType.Archive,
																	 sort: TwitchLib.Api.Core.Enums.VideoSort.Time);

				var response = new AdditionalStreamInfoDto()
				{
					StreamId = info.Videos[0].StreamId,
					Url = info.Videos[0].Url,
					ThumbnailUrl = info.Videos[0].ThumbnailUrl,
				};

				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get additional stream info");
				return null;
			}
		}

	}
}
