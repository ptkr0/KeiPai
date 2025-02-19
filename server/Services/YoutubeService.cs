using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using Interfaces;
using Dtos.Youtube;

namespace Services
{
	public class YoutubeService : IYoutubeService
	{
		private readonly IConfiguration _config;
		private readonly ILogger<IYoutubeService> _logger;

		public YoutubeService(IConfiguration config, ILogger<IYoutubeService> logger)
		{
			_config = config;
			_logger = logger;
		}

		/// <summary>
		/// after user logs in with google through the frontend, the frontend sends the code to the backend 
		/// uses youtube api to verify user and get access token and refresh token
		/// </summary>
		/// <param name="code">code gathered from the user</param>
		/// <param name="userId">id of the user connecting accounts</param>
		/// <returns>access token and refresh token</returns>
		/// <exception cref="InvalidOperationException">appsettings.json is incorrectly set up or scopes are invalid</exception>
		public async Task<ExchangedCodeForTokensDto?> ExchangeCodeForTokens(string code, string userId)
		{
			try
			{
				string clientId = _config["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId is required.");
				string secret = _config["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Google Client Secret is required.");

				var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
				{
					ClientSecrets = new ClientSecrets { ClientId = clientId, ClientSecret = secret },
				});

				var tokenResponse = await flow.ExchangeCodeForTokenAsync(userId, code, _config["Client:Http"] + "/youtube-success", CancellationToken.None);

				if (!tokenResponse.Scope.Split(' ').OrderBy(s => s).SequenceEqual(new[]
				{
					"https://www.googleapis.com/auth/userinfo.email",
					"https://www.googleapis.com/auth/youtube.readonly",
					"openid",
				}))
				{
					return null;
					throw new InvalidOperationException("Invalid scope");
				}

				string accessToken = tokenResponse.AccessToken;

				string refreshToken = tokenResponse.RefreshToken;

				if (refreshToken == null) throw new InvalidOperationException("refresh token is null");

				return new ExchangedCodeForTokensDto
				{
					AccessToken = accessToken,
					RefreshToken = refreshToken
				};
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to exchange code for tokens");
				return null;
			}
		}

		/// <summary>
		/// uses youtube api to get new access token
		/// </summary>
		/// <param name="refreshToken"></param>
		/// <param name="userId"></param>
		/// <returns>new access token</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string?> GetAccessToken(string refreshToken, string userId)
		{
			try
			{
				string clientId = _config["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId is required.");
				string secret = _config["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Google Client Secret is required.");

				var googleAuthorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
				{
					ClientSecrets = new ClientSecrets
					{
						ClientId = clientId,
						ClientSecret = secret
					}
				});

				var newToken = await googleAuthorizationCodeFlow.RefreshTokenAsync(userId, refreshToken, CancellationToken.None);

				return newToken.AccessToken;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to get access token");
				return null;
			}
		}

		/// <summary>
		/// uses youtube api to get channel info
		/// </summary>
		/// <param name="accessToken"></param>
		/// <returns>channel id, name and number of subscribers</returns>
		public async Task<ChannelInfoDto> GetChannelInfo(string accessToken)
		{
			try
			{
				var credential = GoogleCredential.FromAccessToken(accessToken);

				var youtube = new YouTubeService(new BaseClientService.Initializer
				{
					HttpClientInitializer = credential,
					ApplicationName = "KeiPai"
				});

				var request = youtube.Channels.List("snippet,statistics");
				request.Mine = true;

				ChannelListResponse response = await request.ExecuteAsync();

				var channel = response.Items[0];
				string channelId = channel.Id;
				string channelName = channel.Snippet.Title;
				ulong subscriberCount = channel.Statistics.SubscriberCount.GetValueOrDefault(0);

				ChannelInfoDto channelInfo = new ChannelInfoDto
				{
					ChannelId = channelId,
					ChannelName = channelName,
					SubscriberCount = subscriberCount
				};

				return channelInfo;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to get channel info");
				return null!;
			}
		}

		/// <summary>
		/// uses youtube api to get detailed info about all videos of a channel
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="channelId"></param>
		/// <param name="publishedAfter">to reduce number of api calls we can limit publish date</param>
		/// <returns>list of all the videos</returns>
		public async Task<List<YoutubeVideoDto>> GetAllVideos(string accessToken, string channelId, DateTimeOffset? publishedAfter)
		{
			try
			{
				var credential = GoogleCredential.FromAccessToken(accessToken);

				var youtube = new YouTubeService(new BaseClientService.Initializer
				{
					HttpClientInitializer = credential,
					ApplicationName = "KeiPai"
				});

				var nextPageToken = "";
				var listOfAllVideos = new List<YoutubeVideoDto>();

				while (nextPageToken != null)
				{
					var searchListRequest = youtube.Search.List("snippet");
					searchListRequest.ChannelId = channelId; // i think ForMine would work as well
					searchListRequest.MaxResults = 50; // max allowed by youtube api
					searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date; // not necessary
					searchListRequest.PageToken = nextPageToken; // pagination (if more user has more than 50 videos)
					searchListRequest.PublishedAfterDateTimeOffset = publishedAfter; // filter by published date

					var searchListResponse = await searchListRequest.ExecuteAsync();

					// if there are no more videos, break the loop
					if (searchListResponse.Items == null || !searchListResponse.Items.Any())
					{
						break;
					}

					// extract video ids from search results
					var videoIds = searchListResponse.Items
						.Where(item => item.Id.Kind == "youtube#video")
						.Select(item => item.Id.VideoId)
						.ToList();

					// youtube api only returns basic info about videos, we need to get detailed info
					var videoDetailsRequest = youtube.Videos.List("snippet,statistics,status");
					videoDetailsRequest.Id = string.Join(",", videoIds);
					var videoDetailsResponse = await videoDetailsRequest.ExecuteAsync();

					var videoSearchResult = videoDetailsResponse.Items
						.Where(video => video.Status.PrivacyStatus == "public" && video.Snippet.LiveBroadcastContent == "none")
						.Select(video => new YoutubeVideoDto
						{
							Id = video.Id,
							Title = video.Snippet.Title,
							Description = video.Snippet.Description,
							Thumbnail = video.Snippet.Thumbnails.Medium.Url,
							UploadDate = video.Snippet.PublishedAtDateTimeOffset,
							Url = $"https://www.youtube.com/watch?v={video.Id}",
							ViewCount = video.Statistics.ViewCount ?? 0,
							Tags = video.Snippet.Tags ?? new List<string>()
						})
						.OrderByDescending(x => x.UploadDate)
						.ToList();

					// add the current batch of videos to the main list
					listOfAllVideos.AddRange(videoSearchResult);

					nextPageToken = searchListResponse.NextPageToken;
				}

				return listOfAllVideos;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to get all videos");
				return null!;
			}
		}
	}
}
