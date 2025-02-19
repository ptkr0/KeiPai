using Models;

namespace Interfaces
{
	public interface IPlatformRepository
	{
		Task<ICollection<Platform>> GetPlatforms();
		Task<Platform?> GetPlatformById(int platformId);
	}
}
