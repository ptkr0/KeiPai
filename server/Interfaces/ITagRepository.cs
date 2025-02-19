using Dtos.Tag;
using Models;

namespace Interfaces
{
	public interface ITagRepository
	{
		Task<List<Tag>> GetAll();
		Task<Tag?> GetById(int id);
		Task<ICollection<TagDto>> GetByGame(int gameId);
		Task<ICollection<Tag>> GetTagsByGame(int gameId);
	}
}
