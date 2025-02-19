using Data;
using Dtos.Account;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class DeveloperRepository : IDeveloperRepository
	{
		private readonly ApplicationDBContext _context;

		public DeveloperRepository(ApplicationDBContext context)
		{
			_context = context;
		}

		public async Task<bool> AddDeveloperInfoAsync(User user, string websiteUrl)
		{
			var developer = new Developer { UserId = user.Id, WebsiteUrl = websiteUrl };
			_context.Developers.Add(developer);
			var saveResult = await _context.SaveChangesAsync();
			return saveResult > 0;
		}

		public async Task<Developer?> GetDeveloperInfoAsync(User user)
		{
			return await _context.Developers.FirstOrDefaultAsync(d => d.UserId == user.Id);
		}

		public async Task<DeveloperFullInfoDto?> GetDeveloperFullInfo(string userId)
		{
			var dto = new DeveloperFullInfoDto();

			var developer = await _context.Developers.Where(i => i.UserId == userId).Select(i => new DeveloperDto
			{
				Id = i.UserId,
				WebsiteUrl = i.WebsiteUrl ?? string.Empty,
				About = i.User!.About ?? string.Empty,
				ContactEmail = i.User!.ContactEmail ?? string.Empty,
				Username = i.User!.UserName ?? string.Empty,
			}).FirstOrDefaultAsync();

			if (developer != null) dto.Developer = developer;

			var gameCount = await _context.Games.CountAsync(g => g.DeveloperId == userId);
			var campaignCount = await _context.Campaigns.CountAsync(c => c.Game!.DeveloperId == userId);

			dto.GamesAdded = gameCount;
			dto.CampaignsCreated = campaignCount;

			return dto;
		}

		public async Task<UpdateDeveloperDto?> UpdateDeveloper(string userId, UpdateDeveloperDto dto)
		{
			var developer = await _context.Developers.Include(d => d.User).FirstOrDefaultAsync(d => d.UserId == userId);
			if (developer == null) return null;

			if (dto.Username != null)
			{
				developer.User!.UserName = dto.Username;
				developer.User.NormalizedUserName = dto.Username.ToUpper();
			}

			developer.User!.About = dto.About ?? string.Empty;
			developer.User.ContactEmail = dto.ContactEmail;
			developer.WebsiteUrl = dto.Url;

			_context.Developers.Update(developer);
			await _context.SaveChangesAsync();

			return dto;
		}
	}
}
