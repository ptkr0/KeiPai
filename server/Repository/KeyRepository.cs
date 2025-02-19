using Data;
using Dtos.Key;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class KeyRepository : IKeyRepository
	{
		private readonly ApplicationDBContext _context;

		public KeyRepository(ApplicationDBContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> AddKeys(AddKeysDto keys)
		{
			var platform = await _context.Platforms.FirstOrDefaultAsync(p => p.Id == keys.PlatformId);
			if (platform == null) return new NotFoundObjectResult("Invalid Platform");

			var game = await _context.Games.FirstOrDefaultAsync(g =>
				g.Id == keys
					.GameId); // no need to check to if user is authorized to add keys to a game since it was already checked in the controller
			if (game == null) return new NotFoundObjectResult("Invalid Game");

			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					foreach (var key in keys.Keys)
					{
						var keyToAdd = new Key
						{
							GameId = keys.GameId,
							PlatformId = keys.PlatformId,
							Value = key.Key,
						};

						await _context.Keys.AddAsync(keyToAdd);
					}

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return new OkObjectResult("Keys added successfully: " + keys.Keys.Count);
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
					return new BadRequestObjectResult("Failed to add keys");
				}
			}
		}

		public async Task<IActionResult> DeleteKeys(DeleteKeysDto keysDto, User user)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					foreach (var key in keysDto.KeyIds)
					{
						var keyToDelete = await _context.Keys.FirstOrDefaultAsync(k => k.Id == key && k.RequestId == null);
						if (keyToDelete == null) return new NotFoundObjectResult("Invalid Key");

						var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == keyToDelete.GameId);
						if (game == null) return new NotFoundObjectResult("Invalid Game");

						if (game.DeveloperId != user.Id) return new UnauthorizedObjectResult("Unauthorized");

						_context.Keys.Remove(keyToDelete);
					}

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return new OkObjectResult("Keys deleted successfully: " + keysDto.KeyIds.Count);
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
					return new BadRequestObjectResult("Failed to delete keys");
				}
			}
		}

		public async Task<PaginatedKeys> GetByGameId(int gameId, int pageNumber, int pageSize, List<int>? platforms)
		{
			var query = _context.Keys
				.Where(k => k.GameId == gameId && k.RequestId == null);

			if (platforms != null && platforms.Any())
			{
				var platformEntities = await _context.Platforms
					.Where(p => platforms.Contains(p.Id))
					.ToListAsync();

				if (platformEntities.Any())
				{
					var platformIds = platformEntities.Select(p => p.Id).ToList();
					query = query.Where(k => platformIds.Contains((int)k.PlatformId));
				}
			}

			int count = await query.CountAsync();

			var keys = await query
				.Select(key => new KeyDto
				{
					Id = key.Id,
					Value = key.Value,
					GameId = key.GameId,
					PlatformId = key.PlatformId,
					PlatformName = key.Platform!.Name
				})
				.OrderBy(k => k.Id)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var pageAmount = (int)Math.Ceiling((double)count / pageSize);

			var paginatedKeys = new PaginatedKeys
			{
				Keys = keys,
				TotalCount = count,
				TotalPages = pageAmount,
				CurrentPage = pageNumber,
				PageSize = pageSize
			};

			return paginatedKeys;
		}

		public async Task<Dictionary<int, ICollection<PlatformAndCountDto>>> GetCountOfKeysPerGames(ICollection<int> gameIds)
		{
			var platformCounts = await _context.Keys
				.Where(k => gameIds.Contains(k.GameId) && k.RequestId == null)
				.GroupBy(k => new { k.GameId, k.PlatformId, k.Platform!.Name })
				.Select(g => new
				{
					g.Key.GameId,
					g.Key.PlatformId,
					PlatformName = g.Key.Name,
					Count = g.Count()
				})
				.ToListAsync();

			var countsByGame = platformCounts
				.GroupBy(pc => pc.GameId)
				.ToDictionary(
					g => g.Key,
					g => g.Select(pc => new PlatformAndCountDto
					{
						PlatformId = pc.PlatformId,
						PlatformName = pc.PlatformName,
						Count = pc.Count
					}).ToList() as ICollection<PlatformAndCountDto>
				);

			return countsByGame;
		}

		public async Task<int> GetKeyCountForGameForPlatform(int gameId, int platformId)
		{
			return await _context.Keys.CountAsync(k => k.GameId == gameId && k.PlatformId == platformId && k.RequestId == null);
		}
	};
}