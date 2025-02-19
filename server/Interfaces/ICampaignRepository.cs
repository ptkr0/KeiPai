using Dtos.Campaign;
using Models;

namespace Interfaces
{
	public interface ICampaignRepository
	{
		Task<Campaign> AddCampaign(AddCampaignDto campaignDto, Game game);
		Task<Campaign?> GetCampaignByIdForInfluencer(int campaignId);
		Task<Campaign?> GetCampaignByIdForDeveloper(int campaignId, string userId);
		Task<int> GetNumberOfActiveCampaignsForGame(int gameId);
		Task<PaginatedCampaigns> GetCampaignsForDeveloper(string userId, int pageNumber, int pageSize);
		Task<PaginatedCampaigns> GetCampaignsForInfluencer(string? userId, List<int> tagIds, List<int> platformIds, int pageNumber, int pageSize, bool includeComingSoon, string inflId);
		Task<Campaign?> GetCampaignById(int campaignId);
		Task<CampaignDetailsDto?> GetCampaignDetails(int campaignId);
		Task<bool> CloseCampaign(Campaign campaign);
		Task<Campaign?> UpdateCampaign(Campaign campaign, UpdateCampaignDto updateCampaignDto);
		Task<AssignKeysDto> AssignKeys(Campaign campaign, AssignKeysDto assignKeysDto);
		Task<Key?> CheckIfKeyCanBeAssigned(Campaign campaign, int platformId);
		Task<CanRequestDto> CanInfluencerJoinCampaign(int campaignId, string userId);
		Task<ICollection<ActiveCampaignsListForDeveloperDto>> GetActiveCampaignsListForDeveloper(string userId);
		Task<CampaignStatsDto?> GetCampaignStats(int campaignId, string userId);
	}
}
