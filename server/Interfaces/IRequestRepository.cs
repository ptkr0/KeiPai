using Dtos.Request;
using Models;

namespace Interfaces
{
	public interface IRequestRepository
	{
		Task<ICollection<Request>> GetAllRequestsSentByInfluencer(string userId);
		Task<Request> AddRequest(Request request);
		Task<Request?> GetRequestById(int requestId);
		Task<bool> CancelRequest(Request request);
		Task<ICollection<InfluencerRequestDto>> GetRequestsForInfluencer(string userId, string option);
		Task<ICollection<DeveloperRequestDto>> GetRequestsForDeveloper(string userId, List<int> campaignId, string option);
		Task<Request> UpdateRequest(Request request);
		Task<ICollection<Request>> GetActiveRequestsForGameAndMedia(string influencerId, int gameId, string media);
		Task<bool> CheckIfInfluencerHasAnyRequest(string developerId, string influencerId);
	}
}

