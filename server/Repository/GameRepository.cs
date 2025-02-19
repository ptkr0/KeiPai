using Dtos.Tag;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Game = Models.Game;
using Newtonsoft.Json;
using Data;
using Dtos.Game;
using Interfaces;
using Dtos.Key;

namespace Repository;

public class GameRepository : IGameRepository
{
	private readonly ApplicationDBContext _context;
	private readonly IBlobService _blobService;
	private readonly IKeyRepository _keyRepository;
	private readonly ICampaignRepository _campaignRepository;
	private readonly ILogger<IGameRepository> _logger;

	public GameRepository(ApplicationDBContext context, IBlobService blobService, IKeyRepository keyRepository, ILogger<IGameRepository> logger, ICampaignRepository campaignRepository)
	{
		_context = context;
		_blobService = blobService;
		_keyRepository = keyRepository;
		_logger = logger;
		_campaignRepository = campaignRepository;
	}

	public async Task<Game> AddGame(AddGameDto gameDto, string userId)
	{
		using (var transaction = await _context.Database.BeginTransactionAsync())
		{
			try
			{
				// first. create game entity
				var game = new Game
				{
					Name = gameDto.Name,
					Description = gameDto.Description,
					YoutubeTrailer = gameDto.YoutubeTrailer,
					YoutubeTag = gameDto.YoutubeTag,
					TwitchTagId = gameDto.TwitchTagId,
					TwitchTagName = gameDto.TwitchTagName,
					MinimumCPU = gameDto.MinimumCPU,
					MinimumRAM = gameDto.MinimumRAM,
					MinimumGPU = gameDto.MinimumGPU,
					MinimumStorage = gameDto.MinimumStorage,
					MinimumOS = gameDto.MinimumOS,
					PressKit = gameDto.PressKit,
					ReleaseDate = gameDto.ReleaseDate,
					DeveloperId = userId
				};

				string? coverPhotoUrl = null;

				// check if user uploaded cover photo to the game
				if (gameDto.CoverPhoto != null)
				{
					var coverPhoto = await _blobService.UploadFileBlobAsync(gameDto.CoverPhoto);
					coverPhotoUrl = coverPhoto;
				}
				game.CoverPhoto = coverPhotoUrl;

				// if user added tags to his game
				if (gameDto.Tags != null)
				{
					var distinctTags = gameDto.Tags.Distinct().ToList(); // remove duplicate tags

					var tags = await _context.Tags
						.Where(t => distinctTags.Contains(t.Id))
						.ToListAsync().ConfigureAwait(false);

					foreach (var tag in tags)
					{
						game.Tags.Add(tag);
					}
				}

				await _context.Games.AddAsync(game);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
				return game;
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				_logger.LogError(e, "Failed to add game");
				return null!;
			}
		}
	}

	/// <summary>
	/// returns all games associated with a developer
	/// </summary>
	/// <param name="userId"></param>
	/// <returns>list with basic game informations</returns>	
	public async Task<PaginatedGames> GetAll(string userId, int pageSize, int pageNumber, List<int> tagsId)
	{
		var query = _context.Games
				.Where(g => g.DeveloperId == userId);

		if (!tagsId.IsNullOrEmpty())
		{
			query = query.Where(g => g.Tags.Any(tag => tagsId.Contains(tag.Id)));
		}

		var totalCount = await query.CountAsync();
		var games = await query.Select(game => new GameDto
		{
			Id = game.Id,
			Name = game.Name,
			ReleaseDate = game.ReleaseDate,
			Tags = game.Tags.Select(tag => new TagDto { Id = tag.Id, Name = tag.Name }).ToList(),
			DeveloperId = game.DeveloperId,
			CoverPhoto = game.CoverPhoto ?? string.Empty
		})
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.OrderBy(g => g.Id)
			.ToListAsync();

		var pageAmount = (int)Math.Ceiling((double)totalCount / pageSize);
		return new PaginatedGames
		{
			Games = games,
			PageSize = pageSize,
			TotalCount = totalCount,
			CurrentPage = pageNumber,
			TotalPages = pageAmount
		};
	}


	/// <summary>
	/// gets a game by its id, screenshots and tags associated with the game
	/// it also gets the number of keys per platform
	/// </summary>
	/// <param name="gameId"></param>
	/// <param name="userId"></param>
	/// <returns>dto with game info, tags, screenshots, and number of keys</returns>
	public async Task<GameDetailsDto?> GetById(int gameId, string userId)
	{
		var gameDto = await _context.Games
			.Where(g => g.Id == gameId)
			.Select(g => new GameDetailsDto
			{
				Id = g.Id,
				Name = g.Name,
				Description = g.Description,
				YoutubeTrailer = g.YoutubeTrailer!,
				ReleaseDate = g.ReleaseDate,
				YoutubeTag = g.YoutubeTag!,
				TwitchTagId = g.TwitchTagId!,
				TwitchTagName = g.TwitchTagName!,
				MinimumCPU = g.MinimumCPU!,
				MinimumRAM = g.MinimumRAM!,
				MinimumGPU = g.MinimumGPU!,
				MinimumStorage = g.MinimumStorage!,
				MinimumOS = g.MinimumOS!,
				PressKit = g.PressKit!,
				Tags = g.Tags.Select(tag => new TagDto { Id = tag.Id, Name = tag.Name }).ToList(),
				CoverPhoto = g.CoverPhoto!,
				DeveloperId = g.DeveloperId,
				Screenshots = g.Screenshots.Select(s => new ScreenshotDto { Id = s.Id, Screenshot = s.Image }).ToList(),
				DeveloperName = g.Developer!.User!.UserName
			})
			.FirstOrDefaultAsync();

		return gameDto;
	}

	public async Task<Game?> UpdateGame(int id, UpdateGameDto game, string userId)
	{
		var existingGame = await _context.Games.Include(g => g.Tags).FirstOrDefaultAsync(g => g.Id == id && userId == g.DeveloperId);
		if (existingGame == null) return null;

		using (var transaction = await _context.Database.BeginTransactionAsync())
		{
			try
			{
				existingGame.Name = game.Name ?? existingGame.Name;
				existingGame.Description = game.Description ?? existingGame.Description;
				existingGame.YoutubeTrailer = game.YoutubeTrailer ?? existingGame.YoutubeTrailer;
				existingGame.ReleaseDate = game.ReleaseDate ?? existingGame.ReleaseDate;
				existingGame.YoutubeTag = game.YoutubeTag ?? existingGame.YoutubeTag;
				existingGame.TwitchTagId = game.TwitchTagId ?? existingGame.TwitchTagId;
				existingGame.TwitchTagName = game.TwitchTagName ?? existingGame.TwitchTagName;
				existingGame.MinimumCPU = game.MinimumCPU ?? existingGame.MinimumCPU;
				existingGame.MinimumRAM = game.MinimumRAM ?? existingGame.MinimumRAM;
				existingGame.MinimumGPU = game.MinimumGPU ?? existingGame.MinimumGPU;
				existingGame.MinimumStorage = game.MinimumStorage ?? existingGame.MinimumStorage;
				existingGame.MinimumOS = game.MinimumOS ?? existingGame.MinimumOS;
				existingGame.PressKit = game.PressKit ?? existingGame.PressKit;

				existingGame.Tags.Clear();

				var tags = game.Tags!.Distinct().ToList();
				foreach (int tag in tags)
				{
					var tagExists = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tag);
					if (tagExists != null) existingGame.Tags.Add(tagExists);
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
				return existingGame;
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				_logger.LogError(e, "Failed to update game");
				return null;
			}
		}
	}

	public async Task<ActionResult> DeleteGame(Game game)
	{
		if (game == null) return new NotFoundObjectResult("Invalid Game");

		var keys = await _context.Keys.Where(k => k.GameId == game.Id).ToListAsync();

		using (var transaction = await _context.Database.BeginTransactionAsync())
		{
			try
			{
				if (game.CoverPhoto != null)
				{
					await _blobService.DeleteBlobAsync(game.CoverPhoto);
				}

				var screenshots = await _context.GameScreenshots.Where(s => s.GameId == game.Id).ToListAsync();
				foreach (var screenshot in screenshots)
				{
					await _blobService.DeleteBlobAsync(screenshot.Image);
				}
				_context.RemoveRange(screenshots);

				_context.Games.Remove(game);
				_context.Keys.RemoveRange(keys);
				game.Tags.Clear();
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
				var response = new { message = "Game deleted successfully" };
				var jsonResponse = JsonConvert.SerializeObject(response);
				return new OkObjectResult(jsonResponse);
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				_logger.LogError(e, "Failed to delete game");
				var errorResponse = new { message = "Failed to delete game" };
				var jsonErrorResponse = JsonConvert.SerializeObject(errorResponse);
				return new BadRequestObjectResult(jsonErrorResponse);
			}
		}
	}

	/// <summary>
	/// adds/replaces a cover image for the game
	/// </summary>
	/// <param name="game">game that requires new cover</param>
	/// <param name="cover">file with cover</param>
	/// <returns></returns>
	public async Task<UpdatedGameCoverDto> UpdateCover(Game game, IFormFile cover)
	{
		using (var transaction = _context.Database.BeginTransaction())
		{
			try
			{
				// delete old cover photo if it exists
				if (game.CoverPhoto != null)
				{
					await _blobService.DeleteBlobAsync(game.CoverPhoto);
				}

				var coverPhoto = await _blobService.UploadFileBlobAsync(cover);
				game.CoverPhoto = coverPhoto;
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
				return new UpdatedGameCoverDto { GameId = game.Id, CoverPhoto = coverPhoto };
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				_logger.LogError(e, "Failed to update cover photo");
				return null!;
			}
		}
	}

	/// <summary>
	/// adds a number of screenshots to the database and blob storage for a specified game
	/// </summary>
	/// <param name="game">specified game</param>
	/// <param name="screenshots">a list of screenshots</param>
	/// <returns></returns>
	public async Task<AddedScreenshotsDto> AddScreenshotsToGame(Game game, AddScreenshotsToGameDto screenshots)
	{
		AddedScreenshotsDto screenshotsAdded = new AddedScreenshotsDto();
		using (var transaction = _context.Database.BeginTransaction())
		{
			try
			{
				foreach (var s in screenshots.screenshots)
				{
					var src = await _blobService.UploadFileBlobAsync(s);
					var screen = new ScreenshotDto
					{
						Screenshot = src
					};
					var dbscreenshot = new GameScreenshot { Game = game, GameId = game.Id, Image = src };
					_context.GameScreenshots.Add(dbscreenshot);

					screen.Id = dbscreenshot.Id;
					screenshotsAdded.screenshots.Add(screen);
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
			}


			catch (Exception e)
			{
				await transaction.RollbackAsync();
				_logger.LogError(e, "Failed to add screenshots");
				return null!;
			}
		}

		return screenshotsAdded;

	}

	/// <summary>
	/// returns a game by id. returns null if game not found
	/// </summary>
	/// <param name="gameId"></param>
	/// <returns>game model object</returns>
	public async Task<Game?> GetGameById(int gameId, string userId)
	{
		var game = await _context.Games.FirstOrDefaultAsync(u => u.Id == gameId && u.DeveloperId == userId);

		if (game == null) return null;

		return game;
	}

	/// <summary>
	/// returns a number of games uploaded by the user
	/// </summary>
	/// <param name="userId"></param>
	/// <returns>number of games uploaded by the user</returns>
	public async Task<int> GetNumberOfGames(string userId)
	{
		return await _context.Games.CountAsync(g => g.DeveloperId == userId);
	}

	public async Task<ActionResult> DeleteScreenshot(int screenId, string userId)
	{
		var screenshot = await _context.GameScreenshots.Include(g => g.Game)
			.FirstOrDefaultAsync(s => s.Id == screenId && s.Game!.DeveloperId == userId);

		if (screenshot == null)
			return new NotFoundObjectResult("Invalid screenshot");

		using (var transaction = _context.Database.BeginTransaction())
		{
			try
			{
				await _blobService.DeleteBlobAsync(screenshot.Image);
				_context.GameScreenshots.Remove(screenshot);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
				var response = new { message = "Screenshot deleted successfully" };
				var jsonResponse = JsonConvert.SerializeObject(response);
				return new OkObjectResult(jsonResponse);
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				_logger.LogError(e, "Failed to delete screenshot");
				var response = new { message = "Failed to delete screenshot" };
				var jsonResponse = JsonConvert.SerializeObject(response);
				return new BadRequestObjectResult(jsonResponse);
			}
		}
	}

	public async Task<ICollection<Game>?> CheckIfYoutubeTagExists(IList<string> tags)
	{
		var games = await _context.Games
					.Where(g => tags.Contains(g.YoutubeTag!))
					.ToListAsync();

		return games;
	}

	public async Task<ICollection<Game>?> CheckIfTwitchCategoryExists(IList<string> tags)
	{
		var games = await _context.Games
					.Where(g => tags.Contains(g.TwitchTagName!))
					.ToListAsync();

		return games;
	}

	public async Task<ICollection<GameForCampaignDto>> GetByNameForCampaign(string name, string userId)
	{
		var games = await _context.Games
			.Where(g => g.DeveloperId == userId && g.Name.Contains(name))
			.Where(g => _context.Campaigns.Count(c => c.GameId == g.Id && !c.IsClosed && (c.EndDate == null || c.EndDate > DateTime.Now)) < 2)
			.Select(g => new GameForCampaignDto
			{
				Id = g.Id,
				Name = g.Name,
				CoverPhoto = g.CoverPhoto!,
				ReleaseDate = g.ReleaseDate,
				YoutubeTag = g.YoutubeTag!,
				TwitchTagId = g.TwitchTagId,
				TwitchTagName = g.TwitchTagName!,
			})
			.ToListAsync();

		var gameIds = games.Select(g => g.Id).ToList();

		var countsByGame = await _keyRepository.GetCountOfKeysPerGames(gameIds);

		foreach (var game in games)
		{
			if (countsByGame.TryGetValue(game.Id, out var counts)) game.KeysPerPlatform = counts;
			else game.KeysPerPlatform = new List<PlatformAndCountDto>();
		}

		return games;
	}

	public async Task<int?> CheckIfUserIsOwner(int gameId, string userId)
	{
		return await _context.Games
			.Where(g => g.Id == gameId && g.DeveloperId == userId)
			.Select(g => g.Id)
			.FirstOrDefaultAsync();
	}
}