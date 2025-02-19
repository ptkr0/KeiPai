using Data;
using Dtos.Youtube;
using Interfaces;
using Dtos.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class YoutubeRepository : IYoutubeRepository

	{
		private readonly ApplicationDBContext _context;
		private readonly IRequestRepository _requestRepo;
		private readonly ILogger<IYoutubeRepository> _logger;

		public YoutubeRepository(ApplicationDBContext context, ILogger<IYoutubeRepository> logger, IRequestRepository requestRepo)
		{
			_context = context;
			_requestRepo = requestRepo;
			_logger = logger;
		}

		/// <summary>
		/// loops through the list of videos and adds them to the database with the AddVideo method
		/// </summary>
		/// <param name="videos">a list of videos</param>
		/// <returns>"Added [correctly added videos]/[total number of videos] videos</returns>
		public async Task<string> AddVideos(List<AddVideoDto> videos)
		{
			int count = 0;
			foreach (var video in videos)
			{
				await AddVideo(video);
				count++;
			}
			return "Added " + count + "/" + videos.Count + " videos";
		}

		/// <summary>
		/// adds a youtube account to the database
		/// </summary>
		/// <param name="youtubeAccount">youtube account model object</param>
		/// <returns>youtube account model object</returns>
		public async Task<YoutubeAccount> AddYoutubeAccount(YoutubeAccount youtubeAccount)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					await _context.YoutubeAccounts.AddAsync(youtubeAccount);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return youtubeAccount;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to add youtube account");
					return null!;
				}
			}
		}

		/// <summary>
		/// deletes a youtube account, and all the videos associated with it
		/// </summary>
		/// <param name="youtubeAccount">youtube account model object<</param>
		/// <returns>message if account was deleted successfully</returns>
		public async Task<ActionResult> DeleteYoutubeAccount(YoutubeAccount youtubeAccount)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					// remove the youtube account from the database
					_context.YoutubeAccounts.Remove(youtubeAccount);

					// get all the content made by user ids where type is video
					var videos = await _context.Contents
						.Where(x => x.UserId == youtubeAccount.UserId && x.Type == "video")
						.Include(x => x.YoutubeVideo)
						.ToListAsync();

					// get all the requests made by the user where media is youtube and requests are already done
					var requestsWithVideos = await _context.Requests
						.Include(x => x.Content)
						.Where(x => x.InfluencerId == youtubeAccount.UserId && x.Media == "youtube" && x.ContentId != null)
						.ToListAsync();

					// filter out content that was made for a campaign
					var videosToDelete = videos.Where(video => !requestsWithVideos.Any(request => request.ContentId == video.Id)).ToList();

					// remove all the youtube videos and content from the database
					_context.Contents.RemoveRange(videosToDelete);

					// get not fullfilled requests where media is youtube
					var pendingRequestsWithYoutube = await _context.Requests
						.Where(x => x.InfluencerId == youtubeAccount.UserId && x.Media == "youtube" && x.Status == 0)
						.ToListAsync();

					// remove all the requests from the database
					_context.Requests.RemoveRange(pendingRequestsWithYoutube);

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return new OkObjectResult("Account disconnected successfully");
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to disconnect account");
					return new BadRequestObjectResult("Failed to disconnect account");
				}
			}
		}

		/// <summary>
		/// gets all the videos of a user by user id (if any games are mentioned in the video, they are included in the response)
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>a list of all the videos</returns>
		public async Task<YoutubeVideosPaginated> GetAllVideosByUser(string userId, int pageSize, int pageNumber)
		{
			var query = _context.Contents
				.Where(x => x.UserId == userId && x.Type == "video");

			var videos = await query
				.Select(video => new GetYoutubeVideoDto
				{
					Id = video.Id,
					Games = video.Games!.Select(game => new BasicGameDto
					{
						Id = game.Id,
						Name = game.Name
					}).ToList(),
					Video = new YoutubeVideoDto
					{
						Id = video.YoutubeVideo!.VideoId,
						Title = video.YoutubeVideo.Title,
						Description = video.YoutubeVideo.Description,
						Thumbnail = video.YoutubeVideo.Thumbnail,
						UploadDate = video.YoutubeVideo.UploadDate,
						ViewCount = video.YoutubeVideo.ViewCount,
						Url = video.YoutubeVideo.Url
					}
				})
				.OrderByDescending(x => x.Video.UploadDate)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var totalCount = await _context.Contents
				.Where(x => x.UserId == userId && x.Type == "video").CountAsync();

			var pageAmount = (int)Math.Ceiling((double)totalCount / pageSize);

			return new YoutubeVideosPaginated
			{
				CurrentPage = pageNumber,
				TotalCount = totalCount,
				TotalPages = pageAmount,
				Videos = videos,
				PageSize = pageSize
			};
		}

		/// <summary>
		/// gets a youtube account by user id. returns null if the account does not exist
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>youtube account if exists</returns>
		public async Task<YoutubeAccount?> GetYoutubeAccount(string userId)
		{
			return await _context.YoutubeAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
		}

		/// <summary>
		/// gets more detailed information about a youtube account
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>object containing youtube username, url, view count, avg view count, num of subs</returns>
		public async Task<ChannelInfoDetailedDto?> GetYoutubeAccountInfo(string userId)
		{
			var account = await _context.YoutubeAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
			if (account == null) return null;

			var viewCount = await _context.Contents
				.Where(x => x.UserId == userId && x.Type == "video" && x.YoutubeVideo != null)
				.Select(x => (long)x.YoutubeVideo!.ViewCount)
				.SumAsync();

			var videoCount = await _context.Contents
				.CountAsync(x => x.UserId == userId && x.Type == "video");

			var averageViewCount = videoCount > 0 ? viewCount / videoCount : 0;

			return new ChannelInfoDetailedDto
			{
				Username = account.ChannelName,
				Url = "https://youtube.com/channel/" + account.YoutubeChannelId,
				ViewCount = (ulong)viewCount,
				AverageViewCount = (ulong)averageViewCount,
				SubscriberCount = account.SubscriberCount,
				LastCrawlDate = account.LastCrawlDate
			};
		}


		/// <summary>
		/// gets a youtube account by youtube channel id. returns null if the account does not exist
		/// </summary>
		/// <param name="channelId">youtube channel id</param>
		/// <returns>youtube account if exists</returns>
		public async Task<YoutubeAccount?> GetYoutubeAccountByChannelId(string channelId)
		{
			return await _context.YoutubeAccounts.FirstOrDefaultAsync(x => x.YoutubeChannelId == channelId);
		}

		public async Task<YoutubeAccount> UpdateYoutubeAccount(YoutubeAccount youtubeAccount)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					_context.Update(youtubeAccount);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return youtubeAccount;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to add update account");
					return null;
				}
			}
		}

		/// <summary>
		/// used in the background service to get youtube accounts that need to be crawled and their refresh token has not expired
		/// </summary>
		/// <returns>list of eligible youtube accounts</returns>
		public async Task<List<YoutubeAccount>> GetAllYoutubeAccounts()
		{
			// to prevent from crawling the same account multiple times, we only crawl accounts that have not been crawled in the last 2 hours
			var twoHours = DateTimeOffset.Now.AddHours(-2);

			return await _context.YoutubeAccounts.Where(ya => ya.RefreshToken != null && (ya.LastCrawlDate == null || ya.LastCrawlDate <= twoHours)).ToListAsync();
		}

		/// <summary>
		/// adds a video to the database (content entity, youtube video entity, and the games mentioned in the video)
		/// </summary>
		/// <param name="video">video dto</param>
		/// <returns>youtube video model object if created correctly, otherwise null</returns>
		public async Task<Content?> AddVideo(AddVideoDto video)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					// Create a content entity
					var content = new Content
					{
						Type = "video",
						UserId = video.InfluencerId,
					};

					await _context.Contents.AddAsync(content);
					await _context.SaveChangesAsync();

					// Add the games mentioned in the video
					foreach (var game in video.Games)
					{
						content.Games!.Add(game);
					}

					// Create a YouTube video entity
					var youtubeVideo = new YoutubeVideo
					{
						VideoId = video.VideoId,
						Title = video.Title,
						Description = video.Description.Length > 200 ? video.Description.Substring(0, 200) : video.Description,
						Thumbnail = video.Thumbnail,
						UploadDate = video.UploadDate,
						ViewCount = video.ViewCount,
						Url = video.Url,
						ContentId = content.Id
					};

					await _context.YoutubeVideos.AddAsync(youtubeVideo);
					await _context.SaveChangesAsync();

					await transaction.CommitAsync();

					return content;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to add video");
					return null;
				}
			}
		}


		public async Task<YoutubeVideo?> GetVideoById(string videoId, string userId)
		{
			var content = await _context.Contents.Include(c => c.YoutubeVideo).Where(x => x.UserId == userId && x.YoutubeVideo!.VideoId == videoId).FirstOrDefaultAsync();
			return content?.YoutubeVideo;
		}

		/// <summary>
		/// at first finds a video in database, then if the video was found updates it with the new information
		/// it still doesn't update the games mentioned in the video
		/// </summary>
		/// <param name="video">video dto (same as AddVideo)</param>
		/// <returns>youtube video model object if updated correctly, otherwise null</returns>
		public async Task<Content?> UpdateVideo(AddVideoDto video)
		{
			// Find the content in the database
			var content = await _context.Contents
				.Where(x => x.UserId == video.InfluencerId && x.Type == "video")
				.Include(x => x.YoutubeVideo)
				.FirstOrDefaultAsync(x => x.YoutubeVideo!.VideoId == video.VideoId);

			if (content == null) return null;

			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					content.YoutubeVideo!.Title = video.Title ?? content.YoutubeVideo.Title;
					content.YoutubeVideo.Description = video.Description ?? content.YoutubeVideo.Description;
					content.YoutubeVideo.Thumbnail = video.Thumbnail ?? content.YoutubeVideo.Thumbnail;
					content.YoutubeVideo.ViewCount = video.ViewCount;
					_context.YoutubeVideos.Update(content.YoutubeVideo);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return content;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to update video");
					return null;
				}
			}
		}

		public async Task<GetYoutubeVideoDto?> GetVideoDtoById(int videoId)
		{
			return await _context.Contents
				.Where(x => x.Id == videoId)
				.Select(video => new GetYoutubeVideoDto
				{
					Id = video.Id,
					Games = video.Games!.Select(game => new BasicGameDto
					{
						Id = game.Id,
						Name = game.Name
					}).ToList(),
					Video = new YoutubeVideoDto
					{
						Id = video.YoutubeVideo!.VideoId,
						Title = video.YoutubeVideo.Title,
						Description = video.YoutubeVideo.Description,
						Thumbnail = video.YoutubeVideo.Thumbnail,
						UploadDate = video.YoutubeVideo.UploadDate,
						ViewCount = video.YoutubeVideo.ViewCount,
						Url = video.YoutubeVideo.Url
					}
				})
				.FirstOrDefaultAsync();
		}
	}
}
