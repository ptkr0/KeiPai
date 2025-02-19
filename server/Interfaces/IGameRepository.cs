using Dtos.Game;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Interfaces;

public interface IGameRepository
{
	Task<int?> CheckIfUserIsOwner(int gameId, string userId);
	Task<PaginatedGames> GetAll(string userId, int pageSize, int pageNumber, List<int> tagsId);
	Task<GameDetailsDto?> GetById(int gameId, string userId);
	Task<Game?> GetGameById(int gameId, string userId);
	Task<Game> AddGame(AddGameDto game, string userId);
	Task<Game?> UpdateGame(int id, UpdateGameDto game, string userId);
	Task<ActionResult> DeleteGame(Game game);
	Task<UpdatedGameCoverDto> UpdateCover(Game game, IFormFile cover);
	Task<int> GetNumberOfGames(string userId);
	Task<AddedScreenshotsDto> AddScreenshotsToGame(Game game, AddScreenshotsToGameDto screenshots);
	Task<ActionResult> DeleteScreenshot(int screenId, string userId);
	Task<ICollection<Game>?> CheckIfYoutubeTagExists(IList<string> tags);
	Task<ICollection<Game>?> CheckIfTwitchCategoryExists(IList<string> tags);
	Task<ICollection<GameForCampaignDto>> GetByNameForCampaign(string name, string userId);
}