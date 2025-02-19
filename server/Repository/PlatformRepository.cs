using Data;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class PlatformRepository : IPlatformRepository
	{
		private readonly ApplicationDBContext _context;
		public PlatformRepository(ApplicationDBContext context)
		{
			_context = context;
		}
		public async Task<Platform?> GetPlatformById(int platformId)
		{
			var platform = await _context.Platforms.FirstOrDefaultAsync(p => p.Id == platformId);

			if (platform == null) return null;

			return platform;
		}

		public async Task<ICollection<Platform>> GetPlatforms()
		{
			return await _context.Platforms.ToListAsync();
		}
	}
}
