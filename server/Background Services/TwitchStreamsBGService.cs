using Data;
using Interfaces;

namespace Background_Services
{
	public class TwitchStreamsBGService : BackgroundService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public TwitchStreamsBGService(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var timer = new PeriodicTimer(TimeSpan.FromMinutes(2));

			while (await timer.WaitForNextTickAsync(stoppingToken))
			{
				try
				{
					using var scope = _serviceScopeFactory.CreateScope();
					var twitchRepository = scope.ServiceProvider.GetRequiredService<ITwitchRepository>();
					var twitchAccounts = await twitchRepository.GetStreamingAccounts();

					await Parallel.ForEachAsync(twitchAccounts, stoppingToken, async (account, ct) =>
					{
						using var accountScope = _serviceScopeFactory.CreateScope();
						var context = accountScope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
						var twitchRepository = accountScope.ServiceProvider.GetRequiredService<ITwitchRepository>();
						var twitchService = accountScope.ServiceProvider.GetRequiredService<ITwitchService>();

						context.Attach(account);

						// refresh token can't be null since we filter out accounts with empty refresh tokens before
						var tokens = await twitchService.GetAccessToken(account.RefreshToken!);

						if (tokens == null)
						{
							account.RefreshToken = string.Empty;
							await twitchRepository.UpdateTwitchAccount(account);
						}
						else
						{
							account.RefreshToken = tokens.RefreshToken;
							await twitchRepository.UpdateTwitchAccount(account);

							var streamInfo = await twitchService.GetStreamInfo(tokens.AccessToken, account.TwitchChannelId);

							// stream is still going
							if (streamInfo != null)
							{
								await twitchRepository.AddSnapshot(streamInfo);
							}

							// response was null so user has stopped streaming
							else
							{
								// get active stream from database
								var activeStream = await twitchRepository.GetActiveStreamByUsedId(account.UserId);

								// get additional stream info to get url and thumbnail
								var additionalStreamInfo = await twitchService.GetAdditionalStreamInfo(tokens.AccessToken, account.TwitchChannelId);

								// check if proper vod was found
								if (additionalStreamInfo != null && activeStream != null && additionalStreamInfo.StreamId == activeStream.TwitchStream!.StreamId)
								{
									// stop the stream but update the url and thumbnail
									activeStream.TwitchStream.Url = additionalStreamInfo.Url;
									activeStream.TwitchStream.Thumbnail = additionalStreamInfo.ThumbnailUrl;
									await twitchRepository.StopStream(activeStream, account.UserId);
									return;
								}

								// if no vod was found, just stop the stream
								else if (activeStream != null)
								{
									await twitchRepository.StopStream(activeStream, account.UserId);
								}
							}
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
