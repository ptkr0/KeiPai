using Data;
using Interfaces;
using Services;

// this service will run in the background and will scan all twitch accounts every 6 hours
// this service will update the database with follower count and partner/affiliate status
namespace Background_Services
{
	public class TwitchAccountsBGService : BackgroundService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public TwitchAccountsBGService(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var timer = new PeriodicTimer(TimeSpan.FromHours(6));

			while (await timer.WaitForNextTickAsync(stoppingToken))
			{
				try
				{
					using var scope = _serviceScopeFactory.CreateScope();
					var twitchRepository = scope.ServiceProvider.GetRequiredService<ITwitchRepository>();
					var twitchAccounts = await twitchRepository.GetAllTwitchAccounts();

					await Parallel.ForEachAsync(twitchAccounts, stoppingToken, async (account, ct) =>
					{
						using var accountScope = _serviceScopeFactory.CreateScope();
						var context = accountScope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
						var twitchRepository = accountScope.ServiceProvider.GetRequiredService<ITwitchRepository>();
						var twitchService = accountScope.ServiceProvider.GetRequiredService<ITwitchService>();

						context.Attach(account);

						// refresh token can't be null since we filter out accounts with empty refresh tokens before
						var tokens = await twitchService.GetAccessToken(account.RefreshToken!);
						account.LastCrawlDate = DateTime.Now;

						if (tokens == null)
						{
							account.RefreshToken = string.Empty;
							await twitchRepository.UpdateTwitchAccount(account);
						}
						else
						{
							var channelInfo = await twitchService.GetChannelInfo(tokens.AccessToken);

							if (channelInfo == null)
							{
								account.RefreshToken = string.Empty;
								await twitchRepository.UpdateTwitchAccount(account);
							}
							else
							{
								account.RefreshToken = tokens.RefreshToken;
								account.FollowerCount = channelInfo.FollowerCount;
								account.ChannelName = channelInfo.ChannelName;
								account.IsAffiliateOrPartner = channelInfo.IsAffiliateOrPartner;
								await twitchRepository.UpdateTwitchAccount(account);
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
