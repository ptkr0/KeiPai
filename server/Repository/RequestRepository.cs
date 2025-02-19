using Data;
using Dtos.Request;
using Interfaces;
using Dtos.Review;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class RequestRepository : IRequestRepository
	{
		private readonly ApplicationDBContext _context;
		private readonly ILogger<IRequestRepository> _logger;

		public RequestRepository(ApplicationDBContext context, ILogger<IRequestRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<ICollection<Request>> GetAllRequestsSentByInfluencer(string userId)
		{
			return await _context.Requests.Where(r => r.InfluencerId == userId).ToListAsync();
		}

		public async Task<Request> AddRequest(Request request)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					await _context.Requests.AddAsync(request);
					await _context.SaveChangesAsync();

					var key = request.Key;
					if (key != null)
					{
						key.RequestId = request.Id;
						_context.Keys.Update(key);
					}

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return request;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to add request");
					return null!;
				}
			}
		}

		public async Task<Request?> GetRequestById(int requestId)
		{
			return await _context.Requests.FirstOrDefaultAsync(r => r.Id == requestId);
		}

		public async Task<bool> CancelRequest(Request request)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					_context.Requests.Remove(request);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return true;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to cancel request");
					return false;
				}
			}
		}

		public async Task<ICollection<InfluencerRequestDto>> GetRequestsForInfluencer(string userId, string option)
		{
			var query = _context.Requests
				.Where(r => r.InfluencerId == userId);

			if (option == "pending")
			{
				query = query.Where(r => r.Status == 0);
			}
			else if (option == "accepted")
			{
				query = query.Where(r => r.Status == 1 && r.ContentId == null);
			}
			else if (option == "rejected")
			{
				query = query.Where(r => r.Status == 2);
			}
			else if (option == "completed")
			{
				query = query.Where(r => r.Status == 3);
			}

			var requests = await query
				.Select(r => new InfluencerRequestDto
				{
					Id = r.Id,
					CampaignId = r.CampaignId,
					GameId = r.Campaign!.Game!.Id,
					GameName = r.Campaign.Game.Name,
					GameCover = r.Campaign.Game.CoverPhoto!,
					Platform = r.PlatformId,
					Media = r.Media,
					RequestDate = r.RequestDate,
					ConsiderationDate = r.ConsiderationDate,
					Key = r.Key!.Value,
					Status = r.Status,
					Content = r.Content!.Id,
				})
				.OrderByDescending(r => r.ConsiderationDate)
				.OrderByDescending(r => r.RequestDate)
				.ToListAsync();

			return requests;
		}

		public async Task<ICollection<DeveloperRequestDto>> GetRequestsForDeveloper(string userId, List<int> campaignId, string option)
		{
			var query = _context.Requests
				.Where(r => r.Campaign!.Game!.DeveloperId == userId);

			if (campaignId != null && campaignId.Any()) query = query.Where(r => campaignId.Contains(r.CampaignId));

			if (option == "pending")
			{
				query = query.Where(r => r.Status == 0);
			}
			else if (option == "accepted")
			{
				query = query.Where(r => r.Status == 1 && r.ContentId == null);
			}
			else if (option == "rejected")
			{
				query = query.Where(r => r.Status == 2);
			}
			else if (option == "completed")
			{
				query = query.Where(r => r.Status == 3 || r.Status == 1 && r.ContentId != null);
			}
			else if (option == "accepted-and-completed")
			{
				query = query.Where(r => r.Status == 1 || r.Status == 3);
			}

			var requests = await query
				.Select(r => new DeveloperRequestDto
				{
					Id = r.Id,
					CampaignId = r.CampaignId,
					GameId = r.Campaign!.Game!.Id,
					GameName = r.Campaign.Game.Name,
					Platform = r.PlatformId,
					Media = r.Media,
					RequestDate = r.RequestDate,
					ConsiderationDate = r.ConsiderationDate,
					InfluencerName = r.Influencer!.User!.UserName!,
					InfluencerId = r.InfluencerId,
					Language = r.Influencer.Language!,
					InfluencerRating = new UserRatingDto
					{
						TotalRating = r.Influencer.Reviews.Sum(review => (int?)review.Rating) ?? 0,
						NumberOfRatings = r.Influencer.Reviews.Count(),
						UserWasRated = r.Influencer.Reviews.Any(review => review.ReviewerId == userId)
					},
					Status = r.Status,
					Content = r.Content.Id,
				})
				.OrderByDescending(r => r.RequestDate)
				.ToListAsync();

			return requests;
		}

		public async Task<Request> UpdateRequest(Request request)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					_context.Requests.Update(request);
					await _context.SaveChangesAsync();

					var key = request.Key;
					if (key != null)
					{
						key.RequestId = request.Id;
						_context.Keys.Update(key);
					}

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return request;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to update request");
					return null!;
				}
			}
		}

		public async Task<ICollection<Request>> GetActiveRequestsForGameAndMedia(string influencerId, int gameId, string media)
		{
			return await _context.Requests
				.Where(r => r.InfluencerId == influencerId
					&& r.Status == 1
					&& r.ContentId == null
					&& r.Media == media
					&& r.Campaign!.Game!.Id == gameId)
				.ToListAsync();
		}

		public async Task<bool> CheckIfInfluencerHasAnyRequest(string developerId, string influencerId)
		{
			return await _context.Requests
				.AnyAsync(r => r.Campaign!.Game!.DeveloperId == developerId
					&& r.InfluencerId == influencerId
					&& r.Status == 1
					|| r.Status == 3);
		}
	}
}
