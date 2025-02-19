using Dtos.OtherMedia;
using Extensions;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers
{
	[Route("api/othermedia")]
	[ApiController]
	public class OtherMediaController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly IOtherMediaRepository _otherMediaRepo;
		private readonly IBlobService _blobService;
		private readonly IRequestRepository _requestRepo;

		public OtherMediaController(UserManager<User> userManager, IOtherMediaRepository otherMediaRepo, IBlobService blobService, IRequestRepository requestRepo)
		{
			_userManager = userManager;
			_otherMediaRepo = otherMediaRepo;
			_blobService = blobService;
			_requestRepo = requestRepo;
		}

		[Authorize(Roles = "Influencer")]
		[HttpPost("connect")]
		[SwaggerOperation(Summary = "user can link their other platform and provide all necessary informations with form")]
		public async Task<IActionResult> AddOtherPlatform([FromBody] AddOtherMedia otherMediaDto)
		{
			var roles = new[] { "Streamer", "Influencer", "Blogger", "Reporter", "Reviewer", "Press", "Curator", "Freelance Writer" };
			var mediums = new[] { "Website", "Blog", "Facebook Page", "Twitter Profile", "Instagram Profile", "TV", "Radio", "Podcast", "Steam Group", "Magazine" };

			if (!ModelState.IsValid) return BadRequest(ModelState);
			if (!roles.Contains(otherMediaDto.Role)) return BadRequest("Invalid Role");
			if (!mediums.Contains(otherMediaDto.Medium)) return BadRequest("Invalid Medium");

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var platform = await _otherMediaRepo.GetOtherMedia(userId);
			if (platform != null) return BadRequest("Other platform already connected");

			var otherMedia = new OtherMedia
			{
				UserId = userId,
				Name = otherMediaDto.Name,
				Url = otherMediaDto.Url,
				Role = otherMediaDto.Role,
				Medium = otherMediaDto.Medium,
				SampleContent = otherMediaDto.SampleContent,
				IsVerified = false
			};

			otherMedia = await _otherMediaRepo.AddOtherMedia(otherMedia);
			if (otherMedia == null) return BadRequest("Failed to add other platform");

			return Ok(otherMediaDto);
		}

		[Authorize(Roles = "Influencer")]
		[HttpDelete("disconnect")]
		[SwaggerOperation(Summary = "user can remove their other platform and delete all the contents (medias)")]
		public async Task<IActionResult> RemoveOtherPlatform()
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var platform = await _otherMediaRepo.GetOtherMedia(userId);
			if (platform == null) return BadRequest("Platform not found");

			var removed = await _otherMediaRepo.RemoveOtherMedia(platform);
			if (!removed) return BadRequest("Failed to remove platform");

			return Ok(removed);
		}

		[Authorize(Roles = "Moderator")]
		[HttpPut("verify")]
		[SwaggerOperation(Summary = "moderator can verify other platform (if provided good enough evidence)")]
		public async Task<IActionResult> VerifyOtherPlatform([FromBody] string userId)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var platform = await _otherMediaRepo.GetOtherMedia(userId);
			if (platform == null) return BadRequest("Platform not found");

			var verifiedPlatform = await _otherMediaRepo.VerifyMedia(platform);
			if (verifiedPlatform == null) return BadRequest("Failed to verify platform");

			return Ok(platform);
		}

		[Authorize(Roles = "Influencer")]
		[HttpPost("add")]
		[SwaggerOperation(Summary = "user can add content (media) to their other platform")]
		public async Task<IActionResult> AddContent([FromForm] AddContentDto addContentDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (addContentDto.Thumbnail != null)
			{
				List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".webp" };
				string extension = Path.GetExtension(addContentDto.Thumbnail.FileName);
				if (!validExtensions.Contains(extension)) return BadRequest("Invalid file type");

				long size = addContentDto.Thumbnail.Length;
				if (size > 1 * 1024 * 1024) return BadRequest("File size too large");
			}

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var platform = await _otherMediaRepo.GetOtherMedia(userId);
			if (platform == null) return BadRequest("Platform not found");

			var content = await _otherMediaRepo.AddContent(addContentDto, userId);
			if (content == null) return BadRequest("Failed to add content");

			if (addContentDto.GameIds != null)
			{
				foreach (var gameId in addContentDto.GameIds)
				{
					var requests = await _requestRepo.GetActiveRequestsForGameAndMedia(userId, gameId, "other");

					foreach (var request in requests)
					{
						request.ContentId = content.Id;
						request.Status = 3;
						await _requestRepo.UpdateRequest(request);
					}
				}
			}

			return Ok(new
			{
				content.Id,
				content.Type,
				content.OtherContent!.Title,
				content.OtherContent.Description,
				content.OtherContent.Url,
				content.OtherContent.Thumbnail
			}
			);
		}

		[HttpGet("{username}")]
		[Authorize]
		[SwaggerOperation(Summary = "gets basic informations about connected platform")]
		public async Task<IActionResult> GetOtherPlatform(string username)
		{
			var user = await _userManager.FindByNameAsync(username);
			if (user == null) return NotFound();

			var platform = await _otherMediaRepo.GetOtherMedia(user.Id);
			if (platform == null) return NotFound();

			return Ok(platform);
		}

		[HttpGet("content/{userId}")]
		[Authorize]
		[SwaggerOperation(Summary = "gets all media content for a user")]
		public async Task<IActionResult> GetContent(string userId)
		{
			var content = await _otherMediaRepo.GetAllContent(userId);
			if (content == null) return NotFound();

			return Ok(content);
		}

		[HttpGet("content/s/{id}")]
		[Authorize]
		[SwaggerOperation(Summary = "gets media content for a specific id")]
		public async Task<IActionResult> GetContentId(int id)
		{
			var content = await _otherMediaRepo.GetContent(id);
			if (content == null) return NotFound();

			return Ok(content);
		}
	}
}
