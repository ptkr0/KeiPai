using Dtos.Key;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Interfaces
{
	public interface IKeyRepository
	{
		Task<IActionResult> AddKeys(AddKeysDto keys);
		Task<PaginatedKeys> GetByGameId(int GameId, int pageNumber, int pageSize, List<int>? platforms);
		Task<IActionResult> DeleteKeys(DeleteKeysDto keys, User user);
		Task<Dictionary<int, ICollection<PlatformAndCountDto>>> GetCountOfKeysPerGames(ICollection<int> gameIds);
		Task<int> GetKeyCountForGameForPlatform(int gameId, int platformId);
	}
}
