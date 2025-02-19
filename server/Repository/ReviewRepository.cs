using Data;
using Dtos.Review;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class ReviewRepository : IReviewRepository
	{
		private readonly ApplicationDBContext _context;
		private readonly ILogger<IReviewRepository> _logger;

		public ReviewRepository(ApplicationDBContext context, ILogger<IReviewRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<ICollection<GetReviewDto>> GetAllReviewsForReviewee(string revieweeId)
		{
			return await _context.Reviews.Where(r => r.RevieweeId == revieweeId).Select(r => new GetReviewDto
			{
				Id = r.Id,
				ReviewerId = r.IsAnonymous ? null : r.ReviewerId,
				ReviewerName = r.IsAnonymous ? null : r.Reviewer!.User!.UserName,
				Rating = r.Rating,
				Comment = r.Comment,
				ReviewDate = r.ReviewDate,
				IsAnonymous = r.IsAnonymous
			}).ToListAsync();
		}

		public async Task<Review> AddReview(Review review)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					await _context.Reviews.AddAsync(review);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return review;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to add request");
					return null!;
				}
			}
		}

		public async Task<Review> UpdateReview(Review review)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					_context.Reviews.Update(review);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return review;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to update review");
					return null!;
				}
			}
		}

		public async Task<Review?> GetReviewById(string reviewerId, string revieweeId)
		{
			return await _context.Reviews.FirstOrDefaultAsync(r => r.RevieweeId == revieweeId && r.ReviewerId == reviewerId);
		}

		public async Task<bool> DeleteReview(Review review)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					_context.Reviews.Remove(review);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return true;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to delete review");
					return false;
				}
			}
		}

		public async Task<GetReviewDto?> GetReviewForUser(string reviewerId, string revieweeId)
		{
			return await _context.Reviews.Where(r => r.RevieweeId == revieweeId && r.ReviewerId == reviewerId).Select(r => new GetReviewDto
			{
				Id = r.Id,
				Rating = r.Rating,
				Comment = r.Comment,
				ReviewDate = r.ReviewDate,
				IsAnonymous = r.IsAnonymous
			}).FirstOrDefaultAsync();
		}

		public async Task<UserRatingDto> GetUserRating(string userId)
		{
			return await _context.Reviews
				.Where(r => r.RevieweeId == userId)
				.GroupBy(r => r.RevieweeId)
				.Select(g => new UserRatingDto
				{
					TotalRating = g.Sum(r => r.Rating),
					NumberOfRatings = g.Count(),
					UserWasRated = g.Any(r => r.ReviewerId == userId)
				})
				.FirstOrDefaultAsync() ?? new UserRatingDto();
		}
	}
}
