using Dtos.Account;
using Models;

namespace Interfaces
{
	public interface IInfluencerRepository
	{
		Task<bool> AddInfluencerInfo(User user, string language);
		Task<UpdateInfluecnerDto?> UpdateInfluencer(string userId, UpdateInfluecnerDto dto);
		Task<Influencer?> GetInfluencerInfoAsync(User user);
		Task<InfluencerInfoDto?> GetInfluencerInfo(string userId);
		Task<InfluencerFullInfoDto?> GetInfluencerFullInfo(string userId);
	}
}
