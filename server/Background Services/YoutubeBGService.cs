using Data;
using Interfaces;
using Dtos.Youtube;
using Models;

// this service will run in the background and will scan all youtube accounts every 6 hours
// this service looks only for videos uploaded in the last year and will update the database with new videos
namespace Background_Services
{
	public class YoutubeBGService : BackgroundService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public YoutubeBGService(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

			while (await timer.WaitForNextTickAsync(stoppingToken))
			{
				try
				{
					using var scope = _serviceScopeFactory.CreateScope();
					var youtubeRepository = scope.ServiceProvider.GetRequiredService<IYoutubeRepository>();
					var youtubeAccounts = await youtubeRepository.GetAllYoutubeAccounts();

					await Parallel.ForEachAsync(youtubeAccounts, stoppingToken, async (account, ct) =>
					{
						using var accountScope = _serviceScopeFactory.CreateScope();
						var context = accountScope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
						var youtubeRepository = accountScope.ServiceProvider.GetRequiredService<IYoutubeRepository>();
						var youtubeService = accountScope.ServiceProvider.GetRequiredService<IYoutubeService>();
						var gameRepository = accountScope.ServiceProvider.GetRequiredService<IGameRepository>();
						var requestRepository = accountScope.ServiceProvider.GetRequiredService<IRequestRepository>();

						context.Attach(account);

						// refresh token can't be null since we filter out accounts with empty refresh tokens before
						var accessToken = await youtubeService.GetAccessToken(account.RefreshToken!, account.UserId);
						account.LastCrawlDate = DateTime.Now;

						if (accessToken == null)
						{
							account.RefreshToken = string.Empty;
							await youtubeRepository.UpdateYoutubeAccount(account);
						}
						else
						{
							var oneYearAgo = DateTimeOffset.Now.AddYears(-1);
							var videos = await youtubeService.GetAllVideos(accessToken, account.YoutubeChannelId, oneYearAgo);

							foreach (var video in videos)
							{
								var youtubeVideo = await youtubeRepository.GetVideoById(video.Id, account.UserId);

								var videoDto = new AddVideoDto
								{
									Games = await gameRepository.CheckIfYoutubeTagExists(video.Tags) ?? new List<Game>(),
									InfluencerId = account.UserId,
									VideoId = video.Id,
									Title = video.Title,
									Description = video.Description,
									Thumbnail = video.Thumbnail,
									UploadDate = video.UploadDate,
									ViewCount = video.ViewCount,
									Url = $"https://www.youtube.com/watch?v={video.Id}"
								};

								Content? content = null;

								if (youtubeVideo == null)
								{
									content = await youtubeRepository.AddVideo(videoDto);
								}
								else
								{
									content = await youtubeRepository.UpdateVideo(videoDto);
								}

								if (content != null)
								{
									// update requests
									foreach (var game in videoDto.Games)
									{
										var requests = await requestRepository.GetActiveRequestsForGameAndMedia(account.UserId, game.Id, "youtube");

										foreach (var request in requests)
										{
											// check if the video's upload date is after the request's consideration date
											if (request.ContentId == null &&
												request.ConsiderationDate != null &&
												videoDto.UploadDate >= request.ConsiderationDate)
											{
												request.ContentId = content.Id;
												request.Status = 3;
												await requestRepository.UpdateRequest(request);
											}
										}
									}
								}
							}

							var channelInfo = await youtubeService.GetChannelInfo(accessToken);
							account.ChannelName = channelInfo.ChannelName;
							account.SubscriberCount = channelInfo.SubscriberCount;
							account.LastCrawlDate = DateTime.Now;
							await youtubeRepository.UpdateYoutubeAccount(account);
						}
					});
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}
	}

}
