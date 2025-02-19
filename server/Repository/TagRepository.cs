using Data;
using Dtos.Tag;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class TagRepository : ITagRepository
	{
		private readonly ApplicationDBContext _context;
		public TagRepository(ApplicationDBContext context)
		{
			_context = context;
		}

		public async Task<List<Tag>> GetAll()
		{
			return await _context.Tags.ToListAsync();
		}

		public async Task<ICollection<TagDto>> GetByGame(int gameId)
		{
			return await _context.Games
				.Include(g => g.Tags)
				.Where(g => g.Id == gameId)
				.SelectMany(g => g.Tags)
				.Select(tag => new TagDto { Id = tag.Id, Name = tag.Name })
				.ToListAsync();
		}

		public Task<Tag?> GetById(int id)
		{
			return _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
		}

		public async Task<ICollection<Tag>> GetTagsByGame(int gameId)
		{
			return await _context.Games
				.Include(g => g.Tags)
				.Where(g => g.Id == gameId)
				.SelectMany(g => g.Tags)
				.Select(tag => new Tag { Id = tag.Id, Name = tag.Name })
				.ToListAsync();
		}
	}
}
