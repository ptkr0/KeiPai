using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Interfaces;
using Models;
using Dtos.Youtube;
using Extensions;

namespace Controllers
{
	[Route("api/youtube")]
	[ApiController]
	public class YoutubeController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly IYoutubeService _youtubeService;
		private readonly IYoutubeRepository _youtubeRepo;
		private readonly IGameRepository _gameRepository;
		private readonly IRequestRepository _requestRepo;

		public YoutubeController(UserManager<User> userManager, IYoutubeService youtubeService, IYoutubeRepository youtubeRepo, IGameRepository gameRepository, IRequestRepository requestRepo)
		{
			_userManager = userManager;
			_youtubeService = youtubeService;
			_youtubeRepo = youtubeRepo;
			_gameRepository = gameRepository;
			_requestRepo = requestRepo;
		}

		[Authorize(Roles = "Influencer")]
		[HttpPost("connect")]
		[SwaggerOperation(Summary = "uses a string from Google OAuth 2.0 screen to connect youtube account")]
		public async Task<IActionResult> ConnectYoutube([FromBody] string code)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var youtubeAccount = await _youtubeRepo.GetYoutubeAccount(userId);
			if (youtubeAccount != null) return BadRequest("Youtube account already connected");

			var response = await _youtubeService.ExchangeCodeForTokens(code, userId);
			if (response == null) return BadRequest("Failed to exchange code for tokens");

			var channelInfo = await _youtubeService.GetChannelInfo(response.AccessToken);
			if (channelInfo == null) return BadRequest("Failed to get channel info");

			if (await _youtubeRepo.GetYoutubeAccountByChannelId(channelInfo.ChannelId) != null) return BadRequest("Youtube account already connected");

			youtubeAccount = new YoutubeAccount
			{
				UserId = userId,
				RefreshToken = response.RefreshToken,
				YoutubeChannelId = channelInfo.ChannelId,
				ChannelName = channelInfo.ChannelName,
				SubscriberCount = channelInfo.SubscriberCount,
				LastCrawlDate = DateTime.Now
			};

			youtubeAccount = await _youtubeRepo.AddYoutubeAccount(youtubeAccount);
			if (youtubeAccount == null) return BadRequest("Failed to add youtube account");

			var videos = await _youtubeService.GetAllVideos(response.AccessToken, youtubeAccount.YoutubeChannelId, null);

			var videoDtos = new List<AddVideoDto>();
			foreach (var video in videos)
			{
				videoDtos.Add(new AddVideoDto
				{
					Games = await _gameRepository.CheckIfYoutubeTagExists(video.Tags) ?? new List<Game>(),
					InfluencerId = userId,
					VideoId = video.Id,
					Title = video.Title,
					Description = video.Description,
					UploadDate = video.UploadDate,
					Thumbnail = video.Thumbnail,
					ViewCount = video.ViewCount,
					Url = video.Url
				});
			}

			var viewCount = videos.Sum(v => (long)v.ViewCount);
			var avgViewCount = viewCount / videos.Count;

			await _youtubeRepo.AddVideos(videoDtos);

			return Ok(new ChannelInfoDetailedDto
			{
				Username = youtubeAccount.ChannelName,
				SubscriberCount = youtubeAccount.SubscriberCount,
				Url = $"https://www.youtube.com/channel/{youtubeAccount.YoutubeChannelId}",
				LastCrawlDate = youtubeAccount.LastCrawlDate,
				ViewCount = (ulong)viewCount,
				AverageViewCount = (ulong)avgViewCount
			});
		}

		[Authorize(Roles = "Influencer")]
		[HttpDelete("disconnect")]
		[SwaggerOperation(Summary = "disconnects youtube account")]
		public async Task<IActionResult> DisconnectYoutube()
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var youtubeAccount = await _youtubeRepo.GetYoutubeAccount(userId);
			if (youtubeAccount == null) return BadRequest("Youtube account not connected");

			var result = await _youtubeRepo.DeleteYoutubeAccount(youtubeAccount);

			return Ok(result);
		}

		[HttpGet("videos/{userId}")]
		[Authorize]
		[SwaggerOperation(Summary = "gets all videos of a user")]
		public async Task<IActionResult> GetVideos(string userId, int pageNumber = 1)
		{
			int pageSize = 20;

			var videos = await _youtubeRepo.GetAllVideosByUser(userId, pageSize, pageNumber);

			return Ok(videos);
		}

		[HttpGet("video/{id}")]
		[Authorize]
		[SwaggerOperation(Summary = "gets a video by id")]
		public async Task<IActionResult> GetVideo(int id)
		{
			var video = await _youtubeRepo.GetVideoDtoById(id);
			if (video == null) return NotFound();

			return Ok(video);
		}

		[HttpGet("{username}")]
		[Authorize]
		[SwaggerOperation(Summary = "gets youtube account of a user")]
		public async Task<IActionResult> GetYoutubeAccount(string username)
		{
			var user = await _userManager.FindByNameAsync(username);
			if (user == null) return NotFound();

			var youtubeAccount = await _youtubeRepo.GetYoutubeAccountInfo(user.Id);

			return Ok(youtubeAccount);
		}

		[Authorize(Roles = "Influencer")]
		[HttpPost("refresh")]
		[SwaggerOperation(Summary = "refreshes youtube account")]
		public async Task<IActionResult> RefreshYoutubeData()
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var youtubeAccount = await _youtubeRepo.GetYoutubeAccount(userId);
			if (youtubeAccount == null) return BadRequest("Youtube account not connected");

			var accessToken = await _youtubeService.GetAccessToken(youtubeAccount.RefreshToken, userId);
			if (accessToken == null) return BadRequest("Failed to get access token");

			var oneYearAgo = DateTimeOffset.Now.AddYears(-1);
			var videos = await _youtubeService.GetAllVideos(accessToken, youtubeAccount.YoutubeChannelId, oneYearAgo);

			foreach (var video in videos)
			{
				var youtubeVideo = await _youtubeRepo.GetVideoById(video.Id, userId);

				var videoDto = new AddVideoDto
				{
					Games = await _gameRepository.CheckIfYoutubeTagExists(video.Tags) ?? new List<Game>(),
					InfluencerId = userId,
					VideoId = video.Id,
					Title = video.Title,
					Description = video.Description,
					Thumbnail = video.Thumbnail,
					UploadDate = video.UploadDate,
					ViewCount = video.ViewCount,
					Url = $"https://www.youtube.com/watch?v={video.Id}"
				};

				Content? content = null;

				if (youtubeVideo == null)
				{
					content = await _youtubeRepo.AddVideo(videoDto);
				}
				else
				{
					content = await _youtubeRepo.UpdateVideo(videoDto);
				}

				if (content != null)
				{
					// update requests
					foreach (var game in videoDto.Games)
					{
						var requests = await _requestRepo.GetActiveRequestsForGameAndMedia(userId, game.Id, "youtube");

						foreach (var request in requests)
						{
							// check if the video's upload date is after the request's consideration date
							if (request.ContentId == null &&
								request.ConsiderationDate != null &&
								videoDto.UploadDate >= request.ConsiderationDate)
							{
								request.ContentId = content.Id;
								request.Status = 3;
								await _requestRepo.UpdateRequest(request);
							}
						}
					}
				}
			}

			var channelInfo = await _youtubeService.GetChannelInfo(accessToken);

			youtubeAccount.ChannelName = channelInfo.ChannelName;
			youtubeAccount.SubscriberCount = channelInfo.SubscriberCount;
			youtubeAccount.LastCrawlDate = DateTime.Now;

			var response = await _youtubeRepo.UpdateYoutubeAccount(youtubeAccount);
			if (response == null) return BadRequest("Failed to update youtube account");

			return Ok();
		}
	}
}
