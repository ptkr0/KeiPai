using Dtos.Review;
using Extensions;
using Interfaces;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers
{
	[Route("api/review")]
	public class ReviewController : ControllerBase
	{
		private readonly IReviewRepository _reviewRepository;
		private readonly IRequestRepository _requestRepository;

		public ReviewController(IReviewRepository reviewRepository, IRequestRepository requestRepository)
		{
			_reviewRepository = reviewRepository;
			_requestRepository = requestRepository;
		}

		[HttpGet("{revieweeId}")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get all reviews for an influencer left by developers")]
		public async Task<IActionResult> GetAllReviewsForReviewee([FromRoute] string revieweeId)
		{
			var reviews = await _reviewRepository.GetAllReviewsForReviewee(revieweeId);

			return Ok(reviews);
		}

		[HttpGet("user/{revieweeId}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to get review for an influencer left by the authorized developer")]
		public async Task<IActionResult> GetReviewForUser([FromRoute] string revieweeId)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var review = await _reviewRepository.GetReviewForUser(userId, revieweeId);

			return Ok(review);
		}

		[HttpPost]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to add review for an influencer")]
		public async Task<IActionResult> AddReview([FromBody] AddReviewDto reviewdto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var canReview = await _requestRepository.CheckIfInfluencerHasAnyRequest(userId, reviewdto.RevieweeId);
			if (!canReview) return BadRequest("You can't review this user");

			var review = new Review
			{
				RevieweeId = reviewdto.RevieweeId,
				ReviewerId = userId,
				Rating = reviewdto.Rating,
				Comment = reviewdto.Comment,
				IsAnonymous = reviewdto.IsAnonymous
			};

			var addedReview = await _reviewRepository.AddReview(review);

			if (addedReview == null) return BadRequest();

			return Ok(review);
		}

		[HttpPut]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to update review for an influencer")]
		public async Task<IActionResult> UpdateReview([FromBody] AddReviewDto reviewdto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var review = await _reviewRepository.GetReviewById(userId, reviewdto.RevieweeId);
			if (review == null) return NotFound();

			var oldRating = review.Rating;

			if (review.ReviewerId != userId) return Unauthorized();

			review.Rating = reviewdto.Rating;
			review.Comment = reviewdto.Comment;
			review.ReviewDate = DateTime.Now;
			review.IsAnonymous = reviewdto.IsAnonymous;

			var updatedReview = await _reviewRepository.UpdateReview(review);

			if (updatedReview == null) return BadRequest();

			var value = new { updatedReview, oldRating };
			return Ok(value);
		}

		[HttpDelete("{revieweeId}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to delete review for an influencer")]
		public async Task<IActionResult> DeleteReview([FromRoute] string revieweeId)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var review = await _reviewRepository.GetReviewById(userId, revieweeId);
			if (review == null) return NotFound();

			if (review.ReviewerId != userId) return Unauthorized();

			var result = await _reviewRepository.DeleteReview(review);

			return Ok(result);
		}
	}
}
