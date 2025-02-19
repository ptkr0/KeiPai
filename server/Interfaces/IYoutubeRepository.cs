using Dtos.Youtube;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Interfaces
{
	public interface IYoutubeRepository
	{
		Task<YoutubeAccount?> GetYoutubeAccount(string userId);
		Task<List<YoutubeAccount>> GetAllYoutubeAccounts();
		Task<YoutubeAccount> AddYoutubeAccount(YoutubeAccount youtubeAccount);
		Task<YoutubeAccount?> GetYoutubeAccountByChannelId(string channelId);
		Task<ActionResult> DeleteYoutubeAccount(YoutubeAccount youtubeAccount);
		Task<YoutubeAccount> UpdateYoutubeAccount(YoutubeAccount youtubeAccount);
		Task<string> AddVideos(List<AddVideoDto> videos);
		Task<Content?> AddVideo(AddVideoDto video);
		Task<Content?> UpdateVideo(AddVideoDto video);
		Task<YoutubeVideosPaginated> GetAllVideosByUser(string userId, int pageSize, int pageNumber);
		Task<ChannelInfoDetailedDto?> GetYoutubeAccountInfo(string userId);
		Task<YoutubeVideo?> GetVideoById(string videoId, string userId); // for testing purposes i use the same youtube account for some users
		Task<GetYoutubeVideoDto?> GetVideoDtoById(int videoId);
	}
}
