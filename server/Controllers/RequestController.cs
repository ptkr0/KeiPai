using Dtos.Request;
using Extensions;
using Interfaces;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers
{
	[Route("api/request")]
	public class RequestController : ControllerBase
	{
		private readonly IPlatformRepository _platformRepo;
		private readonly ICampaignRepository _campaignRepo;
		private readonly IRequestRepository _requestRepo;
		private readonly IYoutubeRepository _youtubeRepo;
		private readonly IOtherMediaRepository _otherMediaRepo;
		private readonly ITwitchRepository _twitchRepo;

		public RequestController(IPlatformRepository platformRepo, ICampaignRepository campaignRepo, IRequestRepository requestRepo, IYoutubeRepository youtubeRepo, IOtherMediaRepository otherMediaRepo, ITwitchRepository twitchRepo)
		{
			_platformRepo = platformRepo;
			_campaignRepo = campaignRepo;
			_requestRepo = requestRepo;
			_youtubeRepo = youtubeRepo;
			_otherMediaRepo = otherMediaRepo;
			_twitchRepo = twitchRepo;
		}

		[HttpPost]
		[Authorize(Roles = "Influencer")]
		[SwaggerOperation(Summary = "endpoint to send request to developer for a campaign")]
		public async Task<IActionResult> SendRequest([FromBody] SendRequestDto request)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			// check if media type is valid
			var platforms = new[] { "youtube", "twitch", "other" };
			if (!platforms.Contains(request.ContentPlatformType)) return BadRequest("Invalid Content Platform Type");

			// check if request game platform is valid
			var gamePlatform = await _platformRepo.GetPlatformById(request.PlatformId);
			if (gamePlatform == null) return BadRequest("Invalid Game Platform");

			// check if campaign is exists and it didnt end
			var campaign = await _campaignRepo.GetCampaignByIdForInfluencer(request.CampaignId);
			if (campaign == null) return BadRequest("Invalid Campaign");
			if (campaign.EndDate < DateTime.Now || campaign.IsClosed) return BadRequest("Campaign has ended");

			// check if influencer can join campaign
			var otherRequests = await _requestRepo.GetAllRequestsSentByInfluencer(userId);
			var pendingAcceptedRequests = otherRequests.Where(r => r.Status == 1 && r.ContentId == null).ToList();
			if (pendingAcceptedRequests.Count >= 5) return BadRequest("You can't send more than 5 requests at a time");
			if (otherRequests.Any(r => r.CampaignId == request.CampaignId)) return BadRequest("You already sent a request for this campaign");


			var newRequest = new Request
			{
				CampaignId = request.CampaignId,
				InfluencerId = userId,
				PlatformId = request.PlatformId,
				Media = request.ContentPlatformType,
				RequestDate = DateTime.Now,
				Status = 0
			};

			var canRequest = false;
			// check if influencer meets minimum qualifications
			switch (request.ContentPlatformType)
			{
				// 1. youtube
				case "youtube":

					// check if campaign supports youtube
					if (campaign.MinimumYoutubeAvgViews == null && campaign.MinimumYoutubeSubscribers == null) return BadRequest("Campaign doesn't support Youtube");

					// get influencers youtube account info
					var youtubeAccount = await _youtubeRepo.GetYoutubeAccountInfo(userId);
					if (youtubeAccount == null) return BadRequest("Youtube account not connected");

					// check if influencer meets minimum qualifications
					if (campaign.MinimumYoutubeAvgViews != null && (int)youtubeAccount.AverageViewCount < campaign.MinimumYoutubeAvgViews
						&& campaign.MinimumYoutubeSubscribers != null && (int)youtubeAccount.SubscriberCount < campaign.MinimumYoutubeSubscribers)
						return BadRequest("You don't meet minimum qualifications");
					else
						canRequest = true;

					if (campaign.AutoCodeDistribution && canRequest) newRequest.Status = 1;
					break;
				case "twitch":

					// check if campaign supports twitch
					if (campaign.MinimumTwitchAvgViewers == null && campaign.MinimumTwitchFollowers == null) return BadRequest("Campaign doesn't support Twitch");

					var twitchAccount = await _twitchRepo.GetTwitchAccount(userId);
					if (twitchAccount == null) return BadRequest("Twitch account not connected");

					if (campaign.MinimumTwitchAvgViewers != null && 5 < campaign.MinimumTwitchAvgViewers
						&& campaign.MinimumTwitchFollowers != null && twitchAccount.FollowerCount < campaign.MinimumTwitchFollowers)
						return BadRequest("You don't meet minimum qualifications");
					else
						canRequest = true;

					if (campaign.AutoCodeDistribution && canRequest) newRequest.Status = 1;
					break;

				case "other":

					// check if campaign supports other media platforms
					if (campaign.AreThirdPartyWebsitesAllowed == 0) return BadRequest("Other Platforms are not supported");

					// get influencers other platform info
					var otherMedia = await _otherMediaRepo.GetOtherMedia(userId);
					if (otherMedia == null) return BadRequest("Other platform not connected");

					// check if influencer meets minimum qualifications
					if (campaign.AreThirdPartyWebsitesAllowed == 2 && otherMedia.IsVerified) canRequest = true;

					else if (campaign.AreThirdPartyWebsitesAllowed == 1) canRequest = true;
					else return BadRequest("You don't meet minimum qualifications");

					if (campaign.AutoCodeDistribution && canRequest) newRequest.Status = 1;
					break;
				default:
					break;
			}

			var canBeAssigned = await _campaignRepo.CheckIfKeyCanBeAssigned(campaign, request.PlatformId);
			if (canBeAssigned == null) return BadRequest("No keys available for this campaign for this platform");

			if (newRequest.Status == 1)
			{
				var key = canBeAssigned.Id;
				newRequest.KeyId = key;
				newRequest.Key = canBeAssigned;
				newRequest.ConsiderationDate = DateTime.Now;
			}

			var result = await _requestRepo.AddRequest(newRequest);
			if (result == null) return BadRequest("Failed to send request");

			return Ok(new { result.Id, result.Status });
		}

		[HttpDelete]
		[Authorize(Roles = "Influencer")]
		[SwaggerOperation(Summary = "endpoint to cancel a request")]
		public async Task<IActionResult> CancelRequest([FromQuery] int requestId)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var request = await _requestRepo.GetRequestById(requestId);
			if (request == null || request.InfluencerId != userId) return NotFound("Request not found");

			if (request.Status == 1) return BadRequest("Request already accepted");

			var result = await _requestRepo.CancelRequest(request);
			if (!result) return BadRequest("Failed to cancel request");

			return Ok();
		}

		[HttpGet("i")]
		[Authorize(Roles = "Influencer")]
		[SwaggerOperation(Summary = "endpoint to get all requests sent by influencer")]
		public async Task<IActionResult> GetRequests([FromQuery] string option)
		{
			if (option != "pending" && option != "accepted" && option != "rejected" && option != "completed") return BadRequest("Invalid option");

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var requests = await _requestRepo.GetRequestsForInfluencer(userId, option);
			return Ok(requests);
		}

		[HttpGet("d")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to get all requests for a developer")]
		public async Task<IActionResult> GetRequestsForDeveloper([FromQuery] List<int> campaignId, string option)
		{
			if (option != "pending" && option != "accepted" && option != "rejected" && option != "completed" && option != "accepted-and-completed") return BadRequest("Invalid option");

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var requests = await _requestRepo.GetRequestsForDeveloper(userId, campaignId, option);
			return Ok(requests);
		}

		[HttpPatch("{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to accept or reject a request")]
		public async Task<IActionResult> AcceptOrRejectRequest([FromRoute] int id, [FromBody] int decision)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var request = await _requestRepo.GetRequestById(id);
			if (request == null) return NotFound("Request not found");

			var campaign = await _campaignRepo.GetCampaignByIdForDeveloper(request.CampaignId, userId);
			if (campaign == null) return Unauthorized("Unauthorized");

			if (request.Status != 0) return BadRequest("Request already accepted or rejected");

			if (decision == 1)
			{
				var canBeAssigned = await _campaignRepo.CheckIfKeyCanBeAssigned(campaign, request.PlatformId);
				if (canBeAssigned == null) return BadRequest("No keys available for this campaign for this platform");

				request.Status = 1;
				request.ConsiderationDate = DateTime.Now;
				request.KeyId = canBeAssigned.Id;
				request.Key = canBeAssigned;
			}
			else if (decision == 2)
			{
				request.Status = 2;
				request.ConsiderationDate = DateTime.Now;
			}
			else
			{
				return BadRequest("Invalid Status");
			}

			var result = await _requestRepo.UpdateRequest(request);
			if (result == null) return BadRequest("Failed to update request");

			return Ok();
		}
	}
}
