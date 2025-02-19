using Dtos.Review;
using Models;

namespace Interfaces
{
	public interface IReviewRepository
	{
		Task<ICollection<GetReviewDto>> GetAllReviewsForReviewee(string revieweeId);
		Task<Review> AddReview(Review review);
		Task<Review> UpdateReview(Review review);
		Task<Review?> GetReviewById(string reviewerId, string revieweeId);
		Task<bool> DeleteReview(Review review);
		Task<GetReviewDto?> GetReviewForUser(string reviewerId, string revieweeId);
		Task<UserRatingDto> GetUserRating(string userId);
	}
}
