using Dtos.Account;
using Models;

namespace Interfaces
{
	public interface IDeveloperRepository
	{
		Task<bool> AddDeveloperInfoAsync(User user, string websiteUrl);
		Task<Developer?> GetDeveloperInfoAsync(User user);
		Task<DeveloperFullInfoDto?> GetDeveloperFullInfo(string userId);
		Task<UpdateDeveloperDto?> UpdateDeveloper(string userId, UpdateDeveloperDto dto);
	}
}
