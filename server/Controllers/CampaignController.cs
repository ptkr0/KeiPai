using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Dtos.Campaign;
using Swashbuckle.AspNetCore.Annotations;
using Interfaces;
using Models;
using Extensions;

namespace Controllers
{
	[Route("api/campaign")]
	[ApiController]
	public class CampaignController : ControllerBase
	{
		private readonly ICampaignRepository _campaignRepo;
		private readonly IGameRepository _gameRepo;
		private readonly IPlatformRepository _platformRepo;
		private readonly IKeyRepository _keyRepo;
		private readonly UserManager<User> _userManager;

		public CampaignController(ICampaignRepository campaignRepo, UserManager<User> userManager, IGameRepository gameRepo, IPlatformRepository platformRepo, IKeyRepository keyRepo)
		{
			_campaignRepo = campaignRepo;
			_userManager = userManager;
			_gameRepo = gameRepo;
			_platformRepo = platformRepo;
			_keyRepo = keyRepo;
		}

		[HttpPost]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to add campaign for a game. max number of active campaigns per game is 2")]
		public async Task<IActionResult> Add([FromBody] AddCampaignDto campaignDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (campaignDto.EndDate < campaignDto.StartDate) return BadRequest("End date must be after start date");

			if (campaignDto.AreThirdPartyWebsitesAllowed != 0
				&& campaignDto.AreThirdPartyWebsitesAllowed != 1
				&& campaignDto.AreThirdPartyWebsitesAllowed != 2)
				return BadRequest("Invalid value for AreThirdPartyWebsitesAllowed");

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var game = await _gameRepo.GetGameById(campaignDto.GameId, userId);
			if (game == null) return NotFound("Invalid Game");

			// it needs to be tested
			if (game.YoutubeTag == null && (campaignDto.MinimumYoutubeAvgViews != null || campaignDto.MinimumYoutubeSubscribers != null))
				return BadRequest("Game doesn't support Youtube");

			if (game.TwitchTagId == null && (campaignDto.MinimumTwitchAvgViewers != null || campaignDto.MinimumTwitchFollowers != null))
				return BadRequest("Game doesn't support Twitch");

			var numberOfCampaigns = await _campaignRepo.GetNumberOfActiveCampaignsForGame(campaignDto.GameId);
			if (numberOfCampaigns >= 2) return BadRequest("Maximum number of campaigns reached for this game");

			var result = await _campaignRepo.AddCampaign(campaignDto, game);
			if (result == null) return BadRequest("Failed to add campaign");

			var addedCampaignDto = new AddedCampaignDto
			{
				Id = result.Id,
				GameId = result.GameId,
				GameName = result.Game!.Name,
				Description = result.Description!,
				StartDate = result.StartDate,
				EndDate = result.EndDate,
				AreThirdPartyWebsitesAllowed = result.AreThirdPartyWebsitesAllowed,
				MinimumTwitchAvgViewers = result.MinimumTwitchAvgViewers,
				MinimumTwitchFollowers = result.MinimumTwitchFollowers,
				MinimumYoutubeAvgViews = result.MinimumYoutubeAvgViews,
				MinimumYoutubeSubscribers = result.MinimumYoutubeSubscribers,
				AutoCodeDistribution = result.AutoCodeDistribution,
				EmbargoDate = result.EmbargoDate,
			};

			return Ok(addedCampaignDto);
		}

		[HttpGet("d")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to get all campaigns for a developer")]
		public async Task<IActionResult> GetCampaigns([FromQuery] int pageNumber = 1)
		{
			var pageSize = 100;
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var campaigns = await _campaignRepo.GetCampaignsForDeveloper(userId, pageNumber, pageSize);

			return Ok(campaigns);
		}

		[HttpGet("d/active")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to get all active campaigns for a developer")]
		public async Task<IActionResult> GetActiveCampaigns()
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var campaigns = await _campaignRepo.GetActiveCampaignsListForDeveloper(userId);

			return Ok(campaigns);
		}

		[HttpGet("{id}")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get details of a campaign")]
		public async Task<IActionResult> GetCampaign([FromRoute] int id)
		{
			var campaign = await _campaignRepo.GetCampaignDetails(id);
			if (campaign == null) return NotFound("Invalid Campaign");

			return Ok(campaign);
		}

		[HttpPatch("{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to close the campaign")]
		public async Task<IActionResult> Close([FromRoute] int id)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var campaign = await _campaignRepo.GetCampaignById(id);

			if (campaign == null) return NotFound("Invalid Campaign");

			if (campaign.Game!.DeveloperId != userId) return Unauthorized();

			var result = await _campaignRepo.CloseCampaign(campaign);
			if (result == false) return BadRequest("Failed to close campaign " + campaign.Id);

			return Ok(new { message = "Campaign closed successfully" });
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "Endpoint to update an existing campaign")]
		public async Task<IActionResult> Update(int id, [FromForm] UpdateCampaignDto campaignDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (campaignDto.EndDate < campaignDto.StartDate)
				return BadRequest("End date must be after start date");

			if (campaignDto.AreThirdPartyWebsitesAllowed < 0 || campaignDto.AreThirdPartyWebsitesAllowed > 2)
				return BadRequest("Invalid value for AreThirdPartyWebsitesAllowed");

			var userId = User.GetUserId();
			if (userId == null)
				return Unauthorized();

			var campaign = await _campaignRepo.GetCampaignById(id);
			if (campaign == null || campaign.Game.DeveloperId != userId)
				return NotFound("Campaign not found");

			var game = campaign.Game;
			if (game.YoutubeTag == null && (campaignDto.MinimumYoutubeAvgViews != null || campaignDto.MinimumYoutubeSubscribers != null))
				return BadRequest("Game doesn't support YouTube");

			if (game.TwitchTagId == null && (campaignDto.MinimumTwitchAvgViewers != null || campaignDto.MinimumTwitchFollowers != null))
				return BadRequest("Game doesn't support Twitch");

			var result = await _campaignRepo.UpdateCampaign(campaign, campaignDto);
			if (result == null)
				return BadRequest("Failed to update campaign");

			var updatedCampaignDto = new UpdatedCampaignDto
			{
				Id = result.Id,
				GameId = result.GameId,
				GameName = result.Game!.Name,
				Description = result.Description!,
				StartDate = result.StartDate,
				EndDate = result.EndDate,
				AreThirdPartyWebsitesAllowed = result.AreThirdPartyWebsitesAllowed,
				MinimumTwitchAvgViewers = result.MinimumTwitchAvgViewers,
				MinimumTwitchFollowers = result.MinimumTwitchFollowers,
				MinimumYoutubeAvgViews = result.MinimumYoutubeAvgViews,
				MinimumYoutubeSubscribers = result.MinimumYoutubeSubscribers,
				AutoCodeDistribution = result.AutoCodeDistribution,
				EmbargoDate = result.EmbargoDate,
			};

			return Ok(updatedCampaignDto);
		}


		[HttpPost("keys/{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to assign a number of keys to a campaign")]
		public async Task<IActionResult> AssignKeys([FromRoute] int id, [FromBody] AssignKeysDto assignKeysDto)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var platform = await _platformRepo.GetPlatformById(assignKeysDto.PlatformId);
			if (platform == null) return NotFound("Invalid Platform");

			var campaign = await _campaignRepo.GetCampaignById(id);

			if (campaign == null || campaign.IsClosed == true || campaign.EndDate < DateTime.Now) return NotFound("Invalid Campaign");

			if (campaign.Game!.DeveloperId != userId) return Unauthorized();

			var result = await _campaignRepo.AssignKeys(campaign, assignKeysDto);
			if (result == null) return BadRequest("Failed to assign keys to campaign " + campaign.Id);

			var assignedKeysDto = new AssignedKeysDto
			{
				CampaignId = id,
				PlatformName = platform.Name,
				GameId = campaign.GameId,
				GameName = campaign.Game.Name,
				PlatformId = result.PlatformId,
				MaximumNumberOfKeys = result.NumberOfKeys,
				NumberOfKeys = await _keyRepo.GetKeyCountForGameForPlatform(id, result.PlatformId),
				IsUnlimited = result.IsUnlimited
			};

			return Ok(assignedKeysDto);
		}

		[HttpGet("i")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get all campaigns for an influencer")]
		public async Task<IActionResult> GetCampaignsForInfluencer(string? userId, [FromQuery] ICollection<int>? tags, [FromQuery] ICollection<int>? platforms, bool includeComingSoon = false, int pageNumber = 1)
		{
			var inflId = User.GetUserId();
			if (inflId == null) return Unauthorized();

			var pageSize = 20;
			var campaigns = await _campaignRepo.GetCampaignsForInfluencer(userId, [.. tags], [.. platforms], pageNumber, pageSize, includeComingSoon, inflId);

			return Ok(campaigns);
		}

		[HttpGet("c/{id}")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to check if influencer can join campaign")]
		public async Task<IActionResult> CanJoin([FromRoute] int id)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var role = User.GetRole();
			if (role != "Influencer") return Ok(new CanRequestDto { CanRequest = false, ReasonCode = 2, ReasonMessage = "Only influencers can join campaigns" });

			var result = await _campaignRepo.CanInfluencerJoinCampaign(id, userId);
			if (result == null) return BadRequest("Failed to check if influencer can join campaign");

			return Ok(result);
		}

		[HttpGet("stats/{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to get stats of a campaign")]
		public async Task<IActionResult> GetStats([FromRoute] int id)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var stats = await _campaignRepo.GetCampaignStats(id, userId);
			if (stats == null) return NotFound("Invalid Campaign");

			return Ok(stats);
		}
	}
}
