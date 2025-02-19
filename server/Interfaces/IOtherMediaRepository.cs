using Dtos.OtherMedia;
using Models;

namespace Interfaces
{
	public interface IOtherMediaRepository
	{
		Task<OtherMedia> AddOtherMedia(OtherMedia otherMedia);
		Task<OtherMedia?> GetOtherMedia(string userId);
		Task<OtherMedia> VerifyMedia(OtherMedia otherMedia);
		Task<bool> RemoveOtherMedia(OtherMedia otherMedia);
		Task<Content> AddContent(AddContentDto addContentDto, string userId);
		Task<List<OtherContentDto>> GetAllContent(string userId);
		Task<OtherContentDto?> GetContent(int id);
	}
}
