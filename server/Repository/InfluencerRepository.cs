using Data;
using Dtos.Account;
using Interfaces;
using Dtos.OtherMedia;
using Dtos.Twitch;
using Dtos.Youtube;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class InfluencerService : IInfluencerRepository
	{
		private readonly ApplicationDBContext _context;
		private readonly IReviewRepository _reviewRepository;

		public InfluencerService(ApplicationDBContext dbContext, IReviewRepository reviewRepository)
		{
			_context = dbContext;
			_reviewRepository = reviewRepository;
		}

		public async Task<bool> AddInfluencerInfo(User user, string language)
		{
			var influencer = new Influencer
			{
				Language = language,
				UserId = user.Id
			};
			_context.Influencers.Add(influencer);
			var saveResult = await _context.SaveChangesAsync();
			return saveResult > 0;
		}

		public async Task<UpdateInfluecnerDto?> UpdateInfluencer(string userId, UpdateInfluecnerDto dto)
		{
			var influencer = await _context.Influencers.Include(d => d.User).FirstOrDefaultAsync(d => d.UserId == userId);
			if (influencer == null) return null;

			if (dto.Username != null)
			{
				influencer.User!.UserName = dto.Username;
				influencer.User.NormalizedUserName = dto.Username.ToUpper();
			}

			influencer.User!.About = dto.About ?? string.Empty;
			influencer.User.ContactEmail = dto.ContactEmail;
			influencer.Language = dto.Language;

			_context.Influencers.Update(influencer);
			await _context.SaveChangesAsync();

			return dto;
		}

		public async Task<Influencer?> GetInfluencerInfoAsync(User user)
		{
			return await _context.Influencers.FirstOrDefaultAsync(i => i.UserId == user.Id);
		}

		public async Task<InfluencerInfoDto?> GetInfluencerInfo(string userId)
		{
			var dto = new InfluencerInfoDto();

			var youtubeData = await _context.YoutubeAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
			if (youtubeData == null) dto.Youtube = null;

			else
			{
				var viewCount = await _context.Contents
					.Where(x => x.UserId == userId && x.Type == "video" && x.YoutubeVideo != null)
					.Select(x => (long)x.YoutubeVideo!.ViewCount)
					.SumAsync();

				var videoCount = await _context.Contents
					.CountAsync(x => x.UserId == userId && x.Type == "video");

				var averageViewCount = videoCount > 0 ? viewCount / videoCount : 0;

				dto.Youtube = new ChannelInfoDetailedDto
				{
					Username = youtubeData.ChannelName,
					Url = "https://youtube.com/channel/" + youtubeData.YoutubeChannelId,
					SubscriberCount = youtubeData.SubscriberCount,
					ViewCount = (ulong)viewCount,
					AverageViewCount = (ulong)averageViewCount,
					LastCrawlDate = youtubeData.LastCrawlDate,
				};
			}

			var twitchData = await _context.TwitchAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
			if (twitchData == null) dto.Twitch = null;

			else
			{
				var averageTwitchViewers = await _context.Contents
					.Where(x => x.UserId == userId && x.Type == "twitch" && x.TwitchStream != null)
					.Select(x => x.TwitchStream!.AverageViewers)
					.AverageAsync();

				averageTwitchViewers = Math.Round(averageTwitchViewers ?? 0);

				dto.Twitch = new TwitchInfoDto
				{
					Username = twitchData.ChannelName,
					Url = "https://twitch.tv/" + twitchData.ChannelName,
					FollowerCount = twitchData.FollowerCount,
					AverageViewers = (int)averageTwitchViewers,
					IsAffiliateOrPartner = twitchData.IsAffiliateOrPartner,
					LastCrawlDate = twitchData.LastCrawlDate,
				};
			}

			var otherMediaData = await _context.OtherMedias.FirstOrDefaultAsync(x => x.UserId == userId);
			if (otherMediaData == null) dto.OtherMedia = null;

			else
			{
				dto.OtherMedia = new OtherMediaInfoDto
				{
					Name = otherMediaData.Name,
					Url = otherMediaData.Url,
					Role = otherMediaData.Role,
					Medium = otherMediaData.Medium,
					IsVerified = otherMediaData.IsVerified,
				};
			}

			return dto;
		}

		public async Task<InfluencerFullInfoDto?> GetInfluencerFullInfo(string userId)
		{
			var dto = new InfluencerFullInfoDto();

			var basicInfo = await GetInfluencerInfo(userId);
			basicInfo ??= new InfluencerInfoDto();
			dto.Media = basicInfo;

			var influencerData = await _context.Influencers.Where(i => i.UserId == userId).Select(i => new InfluencerDto
			{
				Id = i.UserId,
				Language = i.Language ?? string.Empty,
				About = i.User!.About ?? string.Empty,
				ContactEmail = i.User.ContactEmail ?? string.Empty,
				Username = i.User.UserName ?? string.Empty,
			}).FirstOrDefaultAsync();

			if (influencerData != null) dto.Influencer = influencerData;

			dto.Rating = await _reviewRepository.GetUserRating(userId);

			var requestsSent = await _context.Requests
			.CountAsync(r => r.InfluencerId == userId && (r.Status == 1 || r.Status == 3));

			var requestsDone = await _context.Requests
			.CountAsync(r => r.InfluencerId == userId && (r.Status == 1 && r.ContentId != null || r.Status == 3));

			dto.RequestsSent = requestsSent;
			dto.RequestsDone = requestsDone;

			return dto;
		}
	}
}
