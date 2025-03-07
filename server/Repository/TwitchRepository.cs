using Data;
using Dtos.Twitch;
using Interfaces;
using Dtos.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class TwitchRepository : ITwitchRepository
	{
		private readonly ApplicationDBContext _context;
		private readonly IRequestRepository _requestRepo;
		private readonly IGameRepository _gameRepo;
		private readonly ITwitchService _twitchService;
		private readonly ILogger<ITwitchRepository> _logger;


		public TwitchRepository(ILogger<ITwitchRepository> logger, ApplicationDBContext context, IRequestRepository requestRepo, IGameRepository gameRepo, ITwitchService twitchService)
		{
			_logger = logger;
			_context = context;
			_requestRepo = requestRepo;
			_gameRepo = gameRepo;
			_twitchService = twitchService;
		}

		public async Task<TwitchAccount?> GetTwitchAccount(string userId)
		{
			return await _context.TwitchAccounts.FirstOrDefaultAsync(t => t.UserId == userId);
		}

		public async Task<TwitchAccount> AddTwitchAccount(TwitchAccount twitchAccount)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					await _context.TwitchAccounts.AddAsync(twitchAccount);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return twitchAccount;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to add twitch account");
					return null!;
				}
			}
		}

		public async Task<TwitchAccount> UpdateTwitchAccount(TwitchAccount twitchAccount)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					_context.TwitchAccounts.Update(twitchAccount);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return twitchAccount;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to update twitch account");
					return null!;
				}
			}
		}

		public async Task<ActionResult> DeleteTwitchAccount(TwitchAccount twitchAccount)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					_context.TwitchAccounts.Remove(twitchAccount);

					// get all content that is a twitch stream
					var livestreams = await _context.Contents
						.Where(x => x.UserId == twitchAccount.UserId && x.Type == "twitch")
						.Include(x => x.TwitchStream)
						.ToListAsync();

					// get all requests that are for twitch
					var requestsWithTwitch = await _context.Requests
						.Include(x => x.Content)
						.Where(x => x.InfluencerId == twitchAccount.UserId && x.Media == "twitch" && x.ContentId != null)
						.ToListAsync();

					var livestreamsToDelete = livestreams.Where(livestream => !requestsWithTwitch.Any(request => request.ContentId == livestream.Id));

					_context.Contents.RemoveRange(livestreamsToDelete);

					// get all pending requests maed by the user that are for twitch
					var pendingRequestsWithTwitch = await _context.Requests
						.Where(x => x.InfluencerId == twitchAccount.UserId && x.Media == "twitch" && x.Status == 0)
						.ToListAsync();

					_context.Requests.RemoveRange(pendingRequestsWithTwitch);

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return new OkObjectResult("Account disconnected successfully");
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to delete twitch account");
					return new BadRequestObjectResult("Failed to disconnect account");
				}
			}
		}

		public async Task<TwitchAccount?> GetTwitchAccountByChannelId(string channelId)
		{
			return await _context.TwitchAccounts.FirstOrDefaultAsync(t => t.TwitchChannelId == channelId);
		}

		public async Task<List<TwitchAccount>> GetAllTwitchAccounts()
		{
			var twelveHours = DateTimeOffset.Now.AddHours(-12);

			return await _context.TwitchAccounts.Where(ta => !string.IsNullOrEmpty(ta.RefreshToken) && (ta.LastCrawlDate == null || ta.LastCrawlDate <= twelveHours)).ToListAsync();
		}

		public async Task<Content> StartStream(Content twitchStream)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					await _context.Contents.AddAsync(twitchStream);
					await _context.TwitchStreams.AddAsync(twitchStream.TwitchStream!);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return twitchStream;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to start stream");
					return null!;
				}
			}
		}

		public async Task<TwitchStream> StopStream(Content twitchStream, string userId)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var snapshots = await _context.TwitchStreamSnapshots.Where(ts => ts.StreamId == twitchStream.TwitchStream.StreamId).ToListAsync();

					if (snapshots.Any())
					{

						var avgViewers = snapshots.Select(ts => ts.ViewerCount).Average();
						var peakViewers = snapshots.Max(ts => ts.ViewerCount);

						var playedGames = snapshots.Select(ts => ts.IGDBName).Distinct().ToList();
						var filteredGames = await _gameRepo.CheckIfTwitchCategoryExists(playedGames);

						twitchStream.TwitchStream!.Title = snapshots.First().Title;
						twitchStream.TwitchStream.EndDate = DateTime.Now;
						twitchStream.TwitchStream.PeakViewers = peakViewers;
						twitchStream.TwitchStream.AverageViewers = (int)avgViewers;

						if (filteredGames != null)
						{
							// update requests
							foreach (var game in filteredGames)
							{
								twitchStream.Games!.Add(game);
								var requests = await _requestRepo.GetActiveRequestsForGameAndMedia(userId, game.Id, "twitch");

								foreach (var request in requests)
								{
									// check if the stream's upload date is after the request's consideration date
									if (request.ContentId == null &&
										request.ConsiderationDate != null)
									{
										request.ContentId = twitchStream.Id;
										request.Status = 3;
										_context.Requests.Update(request);
									}
								}
							}
						}

						_context.TwitchStreams.Update(twitchStream.TwitchStream);
						await _context.SaveChangesAsync();

						_context.TwitchStreamSnapshots.RemoveRange(snapshots);
						await _context.SaveChangesAsync();

						await transaction.CommitAsync();

						return twitchStream.TwitchStream;
					}
					else
					{
						twitchStream.TwitchStream!.EndDate = DateTime.Now;
						twitchStream.TwitchStream.PeakViewers = 0;
						twitchStream.TwitchStream.AverageViewers = 0;

						_context.TwitchStreams.Update(twitchStream.TwitchStream);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();
						return twitchStream.TwitchStream;
					}
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to stop stream");
					return null!;
				}
			}
		}

		public async Task<ICollection<TwitchAccount>> GetStreamingAccounts()
		{
			// only accounts that are currently streaming (have a stream with no end date)
			return await _context.TwitchAccounts
					.Where(twitchAccount => twitchAccount.RefreshToken != null &&
						_context.Contents
						.Any(content => content.TwitchStream != null &&
										content.TwitchStream.EndDate == null &&
										content.UserId == twitchAccount.UserId))
					.ToListAsync();
		}

		public async Task<Content?> GetActiveStreamByUsedId(string userId)
		{
			return await _context.Contents
				.Include(x => x.TwitchStream)
				.Where(content => content.UserId == userId && content.Type == "twitch" && content.TwitchStream!.EndDate == null)
				.FirstOrDefaultAsync();
		}

		public async Task<TwitchStreamSnapshot> AddSnapshot(TwitchStreamSnapshot snapshot)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					await _context.TwitchStreamSnapshots.AddAsync(snapshot);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return snapshot;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to add snapshot");
					return null!;
				}
			}
		}

		public async Task<TwitchStreamsPaginated> GetAllStreams(string userId, int pageSize, int pageNumber)
		{
			var query = _context.Contents
			   .Where(x => x.UserId == userId && x.Type == "twitch");

			var streams = await query
				.Select(stream => new GetTwitchStreamDto
				{
					Id = stream.Id,
					Games = stream.Games!.Select(game => new BasicGameDto
					{
						Id = game.Id,
						Name = game.Name
					}).ToList(),
					Stream = new TwitchStreamDto
					{
						Id = stream.TwitchStream!.StreamId,
						Title = stream.TwitchStream.Title ?? "No title available",
						Thumbnail = stream.TwitchStream.Thumbnail ?? string.Empty,
						Url = stream.TwitchStream.Url ?? string.Empty,
						StartDate = stream.TwitchStream.StartDate,
						EndDate = stream.TwitchStream.EndDate ?? DateTime.Now,
						PeakViewers = stream.TwitchStream.PeakViewers ?? 0,
						AverageViewers = stream.TwitchStream.AverageViewers ?? 0
					}
				})
				.OrderByDescending(x => x.Stream.StartDate)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var totalCount = await _context.Contents
				.Where(x => x.UserId == userId && x.Type == "stream").CountAsync();

			var pageAmount = (int)Math.Ceiling((double)totalCount / pageSize);

			return new TwitchStreamsPaginated
			{
				CurrentPage = pageNumber,
				TotalCount = totalCount,
				TotalPages = pageAmount,
				Streams = streams,
				PageSize = pageSize
			};
		}

		public async Task<GetTwitchStreamDto?> GetStreamById(int streamId)
		{
			return await _context.Contents
				.Where(x => x.Id == streamId)
				.Select(stream => new GetTwitchStreamDto
				{
					Id = stream.Id,
					Games = stream.Games!.Select(game => new BasicGameDto
					{
						Id = game.Id,
						Name = game.Name
					}).ToList(),
					Stream = new TwitchStreamDto
					{
						Id = stream.TwitchStream!.StreamId,
						Title = stream.TwitchStream.Title ?? "No title available",
						Thumbnail = stream.TwitchStream.Thumbnail ?? string.Empty,
						Url = stream.TwitchStream.Url ?? string.Empty,
						StartDate = stream.TwitchStream.StartDate,
						EndDate = stream.TwitchStream.EndDate ?? DateTime.Now,
						PeakViewers = stream.TwitchStream.PeakViewers ?? 0,
						AverageViewers = stream.TwitchStream.AverageViewers ?? 0
					}
				})
				.FirstOrDefaultAsync();
		}
	}
}
