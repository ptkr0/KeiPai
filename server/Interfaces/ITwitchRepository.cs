using Dtos.Twitch;
using IGDB;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Interfaces
{
	public interface ITwitchRepository
	{
		Task<TwitchAccount?> GetTwitchAccount(string userId);
		Task<TwitchAccount> AddTwitchAccount(TwitchAccount twitchAccount);
		Task<TwitchAccount> UpdateTwitchAccount(TwitchAccount twitchAccount);
		Task<ActionResult> DeleteTwitchAccount(TwitchAccount twitchAccount);
		Task<TwitchAccount?> GetTwitchAccountByChannelId(string channelId);
		Task<TwitchStreamsPaginated> GetAllStreams(string userId, int pageSize, int pageNumber);
		Task<List<TwitchAccount>> GetAllTwitchAccounts();
		Task<Content> StartStream(Content twitchStream);
		Task<TwitchStream> StopStream(Content twitchStream, string userId);
		Task<ICollection<TwitchAccount>> GetStreamingAccounts();
		Task<Content?> GetActiveStreamByUsedId(string userId);
		Task<TwitchStreamSnapshot> AddSnapshot(TwitchStreamSnapshot snapshot);
		Task<GetTwitchStreamDto?> GetStreamById(int streamId);
	}
}
