using Data;
using Dtos.OtherMedia;
using Interfaces;
using Dtos.Game;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class OtherMediaRepository : IOtherMediaRepository
	{
		private readonly ApplicationDBContext _context;
		private readonly IBlobService _blobService;
		private readonly ILogger<IOtherMediaRepository> _logger;
		public OtherMediaRepository(ApplicationDBContext context, IBlobService blobService, ILogger<IOtherMediaRepository> logger)
		{
			_context = context;
			_blobService = blobService;
			_logger = logger;
		}

		/// <summary>
		/// adds other platform to the database
		/// </summary>
		/// <param name="otherMedia">dto contaning all the informations about the platform</param>
		/// <returns>otherplatform model object</returns>
		public async Task<OtherMedia> AddOtherMedia(OtherMedia otherMedia)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					await _context.OtherMedias.AddAsync(otherMedia);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return otherMedia;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to add other platform");
					return null!;
				}
			}
		}

		/// <summary>
		/// gets other platform by user id (if exists)
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<OtherMedia?> GetOtherMedia(string userId)
		{
			return await _context.OtherMedias.FirstOrDefaultAsync(op => op.UserId == userId);
		}

		public async Task<bool> RemoveOtherMedia(OtherMedia otherMedia)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					_context.OtherMedias.Remove(otherMedia);

					// get all the content made by user ids where type is video
					var contents = await _context.Contents
						.Where(x => x.UserId == otherMedia.UserId && x.Type == "other")
						.Include(x => x.OtherContent)
						.ToListAsync();

					var requestsWithContent = await _context.Requests
						.Include(x => x.Content)
						.Where(x => x.ContentId != null && x.InfluencerId == otherMedia.UserId && x.Media == "other")
						.ToListAsync();

					// filter out content that was made for a campaign
					var contentsToDelete = contents.Where(x => requestsWithContent.All(y => y.ContentId != x.Id)).ToList();

					// remove all the other contents and content from the database
					_context.Contents.RemoveRange(contentsToDelete);

					var pendingRequestsWithOtherMedia = await _context.Requests
						.Where(x => x.InfluencerId == otherMedia.UserId && x.Media == "other" && x.Status == 0)
						.ToListAsync();

					// remove all the pending requests with other media
					_context.Requests.RemoveRange(pendingRequestsWithOtherMedia);

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return true;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to disconnect account");
					return false;
				}
			}
		}

		/// <summary>
		/// changes the isVerified property of the platform to true
		/// </summary>
		/// <param name="platform"></param>
		/// <returns></returns>
		public async Task<OtherMedia> VerifyMedia(OtherMedia platform)
		{
			platform.IsVerified = true;
			await _context.SaveChangesAsync();
			return platform;
		}

		public async Task<Content> AddContent(AddContentDto addContentDto, string userId)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					string? coverPhotoUrl = null;
					if (addContentDto.Thumbnail != null)
					{
						var coverPhoto = await _blobService.UploadFileBlobAsync(addContentDto.Thumbnail);
						coverPhotoUrl = coverPhoto;
					}

					var content = new Content
					{
						Type = "other",
						UserId = userId,
					};

					var games = await _context.Games
						   .Where(g => addContentDto.GameIds!.Contains(g.Id))
						   .ToListAsync();

					foreach (var game in games)
					{
						content.Games!.Add(game);
					}

					await _context.Contents.AddAsync(content);
					await _context.SaveChangesAsync();

					var otherContent = new OtherContent
					{
						ContentId = content.Id,
						Title = addContentDto.Title,
						Description = addContentDto.Description,
						Thumbnail = coverPhotoUrl ?? null,
						Url = addContentDto.Url,
						PublishedAt = DateTimeOffset.Now
					};


					await _context.OtherContents.AddAsync(otherContent);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return content;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to add content");
					return null!;
				}
			}
		}

		/// <summary>
		/// gets all the contents of a user by user id (if any games are mentioned in the content, they are included in the response)
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>a neat dto</returns>
		public async Task<List<OtherContentDto>> GetAllContent(string userId)
		{
			return await _context.Contents
				.Where(x => x.UserId == userId && x.Type == "other")
				.Select(content => new OtherContentDto
				{
					Id = content.Id,
					Games = content.Games!.Select(game => new BasicGameDto
					{
						Id = game.Id,
						Name = game.Name
					}).ToList(),
					Title = content.OtherContent!.Title,
					Description = content.OtherContent!.Description,
					Url = content.OtherContent!.Url,
					Thumbnail = content.OtherContent!.Thumbnail ?? string.Empty
				})
				.OrderByDescending(x => x.Id)
				.ToListAsync();
		}

		public async Task<OtherContentDto?> GetContent(int id)
		{
			return await _context.Contents
				.Where(x => x.Id == id)
				.Select(content => new OtherContentDto
				{
					Id = content.Id,
					Games = content.Games!.Select(game => new BasicGameDto
					{
						Id = game.Id,
						Name = game.Name
					}).ToList(),
					Title = content.OtherContent!.Title,
					Description = content.OtherContent!.Description,
					Url = content.OtherContent!.Url,
					Thumbnail = content.OtherContent!.Thumbnail ?? string.Empty
				}).FirstOrDefaultAsync();
		}
	}
}
