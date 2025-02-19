using Dtos.Game;
using Extensions;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers
{
	[Route("api/game")]
	[ApiController]
	public class GameController : ControllerBase
	{
		private readonly IGameRepository _gameRepo;
		private readonly ICampaignRepository _campaignRepo;
		private readonly UserManager<User> _userManager;

		public GameController(IGameRepository gameRepo, ICampaignRepository campaignRepo, UserManager<User> userManager)
		{
			_userManager = userManager;
			_gameRepo = gameRepo;
			_campaignRepo = campaignRepo;
		}


		[HttpGet("d/{userId}")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get all games uploaded by user")]
		public async Task<IActionResult> GetAll([FromRoute] string userId, [FromQuery] ICollection<int>? tagsId, int pageNumber = 1)
		{
			var pageSize = 100;

			var games = await _gameRepo.GetAll(userId, pageSize, pageNumber, [.. tagsId]);

			return Ok(games);
		}

		[HttpGet("{id}")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get game by id")]
		public async Task<IActionResult> GetById([FromRoute] int id)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var game = await _gameRepo.GetById(id, userId);

			if (game == null) return NotFound();

			return Ok(game);
		}

		[HttpGet("find/{name}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to get game by name used in creating campaign")]
		public async Task<IActionResult> GetByName([FromRoute] string name)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var games = await _gameRepo.GetByNameForCampaign(name, userId);

			if (games == null) return NotFound();

			return Ok(games);
		}

		[HttpPost]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary =
			"endpoint to add game. it adds game, tags associated to it and game cover to storage. max number of games added by user is 100")]
		public async Task<IActionResult> AddGame([FromForm] AddGameDto gameDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			// check if cover photo size and type is valid
			if (gameDto.CoverPhoto != null)
			{
				List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".webp" };
				string extension = Path.GetExtension(gameDto.CoverPhoto.FileName);
				if (!validExtensions.Contains(extension)) return BadRequest("Invalid file type");

				long size = gameDto.CoverPhoto.Length;
				if (size > 1 * 1024 * 1024) return BadRequest("File size too large"); // 1MB
			}

			// for now only 5 tags are allowed
			if (gameDto.Tags?.Count > 5) return BadRequest("You can only have 5 tags");

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var numberOfGames = await _gameRepo.GetNumberOfGames(userId);
			if (numberOfGames >= 100) return BadRequest("Maximum number of games reached");

			var result = await _gameRepo.AddGame(gameDto, userId);
			if (result == null) return BadRequest("Failed to add game");

			var newGameDto = new
			{
				result.Id,
				result.Name,
				result.Description,
				result.ReleaseDate,
				result.CoverPhoto,
				result.MinimumCPU,
				result.MinimumRAM,
				result.MinimumGPU,
				result.MinimumStorage,
				result.MinimumOS,
				result.PressKit,
				result.YoutubeTrailer,
				result.YoutubeTag,
				result.TwitchTagId,
				result.TwitchTagName,
				Tags = result.Tags.Select(t => new { t.Id, t.Name }).ToList()
			};

			return Ok(newGameDto);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to update game.")]
		public async Task<IActionResult> UpdateGame([FromRoute] int id, [FromBody] UpdateGameDto gameDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			if (gameDto.Tags?.Count > 5) return BadRequest("You can only have 5 tags");

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var game = await _gameRepo.UpdateGame(id, gameDto, userId);

			if (game == null) return NotFound();

			var updatedGameDto = new
			{
				game.Id,
				game.Name,
				game.Description,
				game.ReleaseDate,
				game.CoverPhoto,
				game.MinimumCPU,
				game.MinimumRAM,
				game.MinimumGPU,
				game.MinimumStorage,
				game.MinimumOS,
				game.PressKit,
				game.YoutubeTrailer,
				game.YoutubeTag,
				game.TwitchTagId,
				game.TwitchTagName,
				Tags = game.Tags.Select(t => new { t.Id, t.Name }).ToList()
			};

			return Ok(updatedGameDto);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to delete game. it deletes game, tags associated to it and game cover from storage.")]
		public async Task<IActionResult> DeleteGame([FromRoute] int id)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var game = await _gameRepo.GetGameById(id, userId);
			if (game == null) return NotFound();

			var campaign = await _campaignRepo.GetNumberOfActiveCampaignsForGame(id);
			if (campaign > 0) return BadRequest("Cannot delete game with active campaigns");

			var result = await _gameRepo.DeleteGame(game);

			return result;
		}

		[HttpPut("cover/{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to update game cover.")]
		public async Task<IActionResult> UpdateCover([FromRoute] int id, [FromForm] UpdateGameCoverDto coverDto)
		{
			if (coverDto.CoverPhoto == null) return BadRequest("No file provided");

			List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".webp" };
			string extension = Path.GetExtension(coverDto.CoverPhoto.FileName);
			if (!validExtensions.Contains(extension)) return BadRequest("Invalid file type");

			long size = coverDto.CoverPhoto.Length;
			if (size > 1 * 1024 * 1024) return BadRequest("File size too large");

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var game = await _gameRepo.GetGameById(id, userId);
			if (game == null) return NotFound();

			var result = await _gameRepo.UpdateCover(game, coverDto.CoverPhoto);

			if (result == null) return BadRequest("There was an error when updating a cover");

			return Ok(game);
		}

		[HttpPost("screenshot/{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to add screenshots")]
		public async Task<IActionResult> AddScreenshotsToGame([FromRoute] int id,
			[FromForm] AddScreenshotsToGameDto screenshots)
		{
			if (screenshots.screenshots.IsNullOrEmpty()) return BadRequest("No file provided");
			if (screenshots.screenshots.Count > 9) return BadRequest("Too many files");

			List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".webp" };

			foreach (var screen in screenshots.screenshots)
			{
				string extension = Path.GetExtension(screen.FileName);
				long size = screen.Length;
				if (!validExtensions.Contains(extension)) return BadRequest("Invalid file type");
				if (size > 1 * 1024 * 1024) return BadRequest("File size too large");

			}

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var game = await _gameRepo.GetGameById(id, userId);
			if (game == null) return NotFound();

			var src = await _gameRepo.AddScreenshotsToGame(game, screenshots);
			if (src == null) return NotFound();

			return Ok(src);
		}

		[HttpDelete("screenshot/{id}")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to delete a screenshot")]
		public async Task<IActionResult> DeleteScreenshotFromGame([FromRoute] int id)
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var gameSrc = await _gameRepo.DeleteScreenshot(id, userId);

			return gameSrc;
		}
	}

}

