using Data;
using Dtos.Campaign;
using Interfaces;
using Dtos.Game;
using Dtos.Tag;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
	public class CampaignRepository : ICampaignRepository
	{
		private readonly ApplicationDBContext _context;
		private readonly ILogger<ICampaignRepository> _logger;

		public CampaignRepository(ApplicationDBContext context, ILogger<ICampaignRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<Campaign> AddCampaign(AddCampaignDto campaignDto, Game game)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var campaign = new Campaign
					{
						Description = campaignDto.Description,
						StartDate = campaignDto.StartDate,
						EndDate = campaignDto.EndDate,
						AreThirdPartyWebsitesAllowed = campaignDto.AreThirdPartyWebsitesAllowed,
						MinimumTwitchAvgViewers = campaignDto.MinimumTwitchAvgViewers,
						MinimumTwitchFollowers = campaignDto.MinimumTwitchFollowers,
						MinimumYoutubeAvgViews = campaignDto.MinimumYoutubeAvgViews,
						MinimumYoutubeSubscribers = campaignDto.MinimumYoutubeSubscribers,
						GameId = game.Id,
						CreationDate = DateTime.Now,
						AutoCodeDistribution = campaignDto.AutoCodeDistribution,
						EmbargoDate = campaignDto.EmbargoDate,
						IsClosed = false
					};
					await _context.Campaigns.AddAsync(campaign);
					await _context.SaveChangesAsync();

					foreach (var key in campaignDto.Keys)
					{
						var campaignKey = new CampaignKey
						{
							CampaignId = campaign.Id,
							PlatformId = key.PlatformId,
							MaximumNumberOfKeys = key.NumberOfKeys,
							IsUnlimited = key.IsUnlimited
						};
						await _context.CampaignKeys.AddAsync(campaignKey);
					}

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return campaign;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Error while adding campaign");
					return null!;
				}
			}
		}

		public async Task<AssignKeysDto> AssignKeys(Campaign campaign, AssignKeysDto assignKeysDto)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					// find if the campaign already has keys assigned for the platform
					var campaignKeys = await _context.CampaignKeys.FirstOrDefaultAsync(ck => ck.CampaignId == campaign.Id && ck.PlatformId == assignKeysDto.PlatformId);

					if (campaignKeys == null) // if not create a new entry and fill it with data
					{
						campaignKeys = new CampaignKey
						{
							CampaignId = campaign.Id,
							PlatformId = assignKeysDto.PlatformId,
							MaximumNumberOfKeys = assignKeysDto.NumberOfKeys,
							IsUnlimited = assignKeysDto.IsUnlimited
						};
						await _context.CampaignKeys.AddAsync(campaignKeys);
					}
					else // if it already exists update the data
					{
						campaignKeys.MaximumNumberOfKeys = assignKeysDto.NumberOfKeys;
						campaignKeys.IsUnlimited = assignKeysDto.IsUnlimited;
					}

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return assignKeysDto;
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		public async Task<bool> CloseCampaign(Campaign campaign)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					campaign.IsClosed = true; // close the campaign
					var requests = await _context.Requests.Where(r => r.CampaignId == campaign.Id).ToListAsync(); // get all the requests for the campaign
					requests.ForEach(r => r.Status = 2); // close all the requests
					_context.Requests.UpdateRange(requests); // update the requests to be closed
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return true;
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
					return false;
				}
			}
		}

		public async Task<Campaign?> GetCampaignById(int campaignId)
		{
			return await _context.Campaigns.Include(c => c.Game).FirstOrDefaultAsync(c => c.Id == campaignId);
		}

		public async Task<Campaign?> GetCampaignByIdForDeveloper(int campaignId, string userId)
		{
			return await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == campaignId && c.Game!.DeveloperId == userId);
		}

		public async Task<Campaign?> GetCampaignByIdForInfluencer(int campaignId)
		{
			return await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == campaignId && (c.EndDate > DateTime.Now || c.EndDate == null) && !c.IsClosed);
		}

		public async Task<CampaignDetailsDto?> GetCampaignDetails(int campaignId)
		{
			return await _context.Campaigns
				.Where(c => c.Id == campaignId)
				.Select(c => new CampaignDetailsDto
				{
					Id = c.Id,
					Game = new GameDetailsDto
					{
						Id = c.Game!.Id,
						Name = c.Game.Name,
						ReleaseDate = c.Game.ReleaseDate,
						Tags = c.Game.Tags.Select(tag => new TagDto
						{
							Id = tag.Id,
							Name = tag.Name
						}).ToList(),
						Description = c.Game.Description ?? string.Empty,
						YoutubeTag = c.Game.YoutubeTag ?? string.Empty,
						YoutubeTrailer = c.Game.YoutubeTrailer ?? string.Empty,
						TwitchTagId = c.Game.TwitchTagId,
						TwitchTagName = c.Game.TwitchTagName ?? string.Empty,
						DeveloperId = c.Game.DeveloperId,
						CoverPhoto = c.Game.CoverPhoto ?? string.Empty,
						MinimumCPU = c.Game.MinimumCPU ?? string.Empty,
						MinimumGPU = c.Game.MinimumGPU ?? string.Empty,
						MinimumOS = c.Game.MinimumOS ?? string.Empty,
						MinimumRAM = c.Game.MinimumRAM ?? string.Empty,
						MinimumStorage = c.Game.MinimumStorage ?? string.Empty,
						PressKit = c.Game.PressKit ?? string.Empty,
						Screenshots = c.Game.Screenshots.Select(s => new ScreenshotDto
						{
							Id = s.Id,
							Screenshot = s.Image
						}).ToList(),
						DeveloperName = c.Game.Developer!.User!.UserName
					},
					StartDate = c.StartDate,
					EndDate = c.EndDate,
					Description = c.Description ?? string.Empty,
					MinimumYoutubeSubscribers = c.MinimumYoutubeSubscribers,
					MinimumTwitchFollowers = c.MinimumTwitchFollowers,
					MinimumTwitchAvgViewers = c.MinimumTwitchAvgViewers,
					MinimumYoutubeAvgViews = c.MinimumYoutubeAvgViews,
					AutoCodeDistribution = c.AutoCodeDistribution,
					EmbargoDate = c.EmbargoDate,
					AreThirdPartyWebsitesAllowed = c.AreThirdPartyWebsitesAllowed,
					IsClosed = c.IsClosed,
					KeysLeftForCampaigns = c.Keys.Select(k => new KeysLeftForCampaignDto
					{
						Id = k.PlatformId,
						Name = k.Platform!.Name,
						CanBeRequested =
							k.IsUnlimited && _context.Keys.Any(key => key.PlatformId == k.PlatformId && key.GameId == c.GameId && key.RequestId == null)
							||
							!k.IsUnlimited &&
								_context.Requests.Count(r => r.CampaignId == c.Id && r.PlatformId == k.PlatformId && r.KeyId != null) < k.MaximumNumberOfKeys
								&& _context.Keys.Any(key => key.PlatformId == k.PlatformId && key.GameId == c.GameId && key.RequestId == null)


					}).ToList()
				})
				.FirstOrDefaultAsync();
		}

		public async Task<PaginatedCampaigns> GetCampaignsForDeveloper(string userId, int pageNumber, int pageSize)
		{
			var totalCount = await _context.Campaigns.Where(c => c.Game!.DeveloperId == userId).CountAsync();
			var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			var campaignsDto = await _context.Campaigns
				.Where(c => c.Game!.DeveloperId == userId)
				.Select(c => new CampaignDto
				{
					Id = c.Id,
					Game = new GameDto
					{
						Id = c.Game!.Id,
						Name = c.Game.Name,
						ReleaseDate = c.Game.ReleaseDate,
						Tags = c.Game.Tags.Select(tag => new TagDto
						{
							Id = tag.Id,
							Name = tag.Name
						}).ToList(),
						DeveloperId = c.Game.DeveloperId,
						CoverPhoto = c.Game.CoverPhoto ?? string.Empty
					},
					StartDate = c.StartDate,
					EndDate = c.EndDate,
					IsClosed = c.IsClosed,
					KeysLeftForCampaign = c.Keys.Select(k => new KeysLeftForCampaignDto
					{
						Id = k.PlatformId,
						Name = k.Platform!.Name,
						CanBeRequested =
							k.IsUnlimited && c.Game.Keys.Any(key => key.PlatformId == k.PlatformId && key.RequestId == null && key.GameId == c.GameId)
							||
							!k.IsUnlimited &&
								c.Requests.Count(r => r.CampaignId == c.Id && r.PlatformId == k.PlatformId && r.KeyId != null) < k.MaximumNumberOfKeys
								&& c.Game.Keys.Any(key => key.PlatformId == k.PlatformId && key.RequestId == null && key.GameId == c.GameId)


					}).ToList()
				})
				.OrderByDescending(c => c.Id)
				.ToListAsync();

			var paginatedCampaigns = new PaginatedCampaigns
			{
				Campaigns = campaignsDto,
				TotalCount = totalCount,
				TotalPages = totalPages,
				PageSize = pageSize
			};

			return paginatedCampaigns;
		}


		public async Task<PaginatedCampaigns> GetCampaignsForInfluencer(string? userId, List<int> tagIds, List<int> platformIds, int pageNumber, int pageSize, bool includeComingSoon, string inflId)
		{
			var query = _context.Campaigns.AsQueryable().Where(c => !c.IsClosed && (c.EndDate == null || c.EndDate > DateTime.Now));

			if (!includeComingSoon)
			{
				query = query.Where(c => c.StartDate <= DateTime.Now);
			}

			if (tagIds != null && tagIds.Any())
			{
				query = query.Where(c => c.Game!.Tags.Any(t => tagIds.Contains(t.Id)));
			}

			if (platformIds != null && platformIds.Any())
			{
				query = query.Where(c => c.Keys.Any(k => platformIds.Contains(k.PlatformId)));
			}

			if (userId != null)
			{
				query = query.Where(c => c.Game!.DeveloperId == userId);
			}

			var totalCount = await query.CountAsync();
			var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			var campaignsDto = await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.Select(c => new CampaignDto
				{
					Id = c.Id,
					Game = new GameDto
					{
						Id = c.Game!.Id,
						Name = c.Game.Name,
						ReleaseDate = c.Game.ReleaseDate,
						Tags = c.Game.Tags.Select(tag => new TagDto
						{
							Id = tag.Id,
							Name = tag.Name
						}).ToList(),
						DeveloperId = c.Game.DeveloperId,
						DeveloperName = c.Game.Developer!.User!.UserName,
						CoverPhoto = c.Game.CoverPhoto ?? string.Empty
					},
					StartDate = c.StartDate,
					EndDate = c.EndDate,
					IsClosed = c.IsClosed,
					DidJoin = c.Requests.Any(r => r.CampaignId == c.Id && r.InfluencerId == inflId),
					KeysLeftForCampaign = c.Keys.Select(k => new KeysLeftForCampaignDto
					{
						Id = k.PlatformId,

						Name = k.Platform!.Name,
						CanBeRequested =
							k.IsUnlimited && c.Game.Keys.Any(key => key.PlatformId == k.PlatformId && key.RequestId == null && key.GameId == c.GameId)
							||
							!k.IsUnlimited &&
								c.Requests.Count(r => r.CampaignId == c.Id && r.PlatformId == k.PlatformId && r.KeyId != null) < k.MaximumNumberOfKeys
								&&
								c.Game.Keys.Any(key => key.PlatformId == k.PlatformId && key.RequestId == null && key.GameId == c.GameId)


					}).ToList()
				})
				.OrderBy(c => c.Game.Name)
				.ToListAsync();

			var paginatedCampaigns = new PaginatedCampaigns
			{
				Campaigns = campaignsDto,
				TotalCount = totalCount,
				TotalPages = totalPages,
				CurrentPage = pageNumber,
				PageSize = pageSize
			};

			return paginatedCampaigns;
		}


		public async Task<int> GetNumberOfActiveCampaignsForGame(int gameId)
		{
			// we only care about campaigns that are not closed and have not ended yet
			return await _context.Campaigns.CountAsync(c => c.GameId == gameId && !c.IsClosed && c.EndDate > DateTime.Now);
		}

		public async Task<Campaign?> UpdateCampaign(Campaign campaign, UpdateCampaignDto campaignDto)
		{
			try
			{
				campaign.Description = campaignDto.Description;
				campaign.StartDate = campaignDto.StartDate;
				campaign.EndDate = campaignDto.EndDate;
				campaign.AreThirdPartyWebsitesAllowed = campaignDto.AreThirdPartyWebsitesAllowed;
				campaign.MinimumTwitchAvgViewers = campaignDto.MinimumTwitchAvgViewers;
				campaign.MinimumTwitchFollowers = campaignDto.MinimumTwitchFollowers;
				campaign.MinimumYoutubeAvgViews = campaignDto.MinimumYoutubeAvgViews;
				campaign.MinimumYoutubeSubscribers = campaignDto.MinimumYoutubeSubscribers;
				campaign.AutoCodeDistribution = campaignDto.AutoCodeDistribution;
				campaign.EmbargoDate = campaignDto.EmbargoDate;

				// Save changes
				await _context.SaveChangesAsync();

				return campaign;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error while updating campaign");
				return null;
			}
		}

		public async Task<Key?> CheckIfKeyCanBeAssigned(Campaign campaign, int platformId)
		{
			// 1. check if the campaign has keys assigned for the platform
			var campaignKey = await _context.CampaignKeys.FirstOrDefaultAsync(ck => ck.CampaignId == campaign.Id && ck.PlatformId == platformId);
			if (campaignKey == null) return null;

			// 2. check if the campaign has reached the maximum number of keys for the platform
			if (!campaignKey.IsUnlimited)
			{
				var numberOfGivenKeys = await _context.Requests.Where(r => r.CampaignId == campaign.Id && r.PlatformId == platformId && r.Status == 1).CountAsync();
				if (numberOfGivenKeys >= campaignKey.MaximumNumberOfKeys) return null;
			}

			// 3. check if the there is an available key for the platform
			return await _context.Keys.FirstOrDefaultAsync(k => k.PlatformId == platformId && k.GameId == campaign.GameId && k.RequestId == null);
		}

		public async Task<CanRequestDto> CanInfluencerJoinCampaign(int campaignId, string userId)
		{
			// check if already joined campainged
			var existingRequest = await _context.Requests.FirstOrDefaultAsync(r => r.CampaignId == campaignId && r.InfluencerId == userId);

			if (existingRequest != null) return new CanRequestDto { CanRequest = false, ReasonCode = 0, ReasonMessage = "User has already sent request to this campaign" };

			// check if user has reached the max request limit (5 in total for pending and accpeted requests)
			var requestCount = await _context.Requests.Where(r => r.InfluencerId == userId && (r.Status == 0 || r.Status == 1 && r.ContentId == null)).CountAsync();

			if (requestCount >= 5) return new CanRequestDto { CanRequest = false, ReasonCode = 1, ReasonMessage = "User reached max request limit" };

			return new CanRequestDto { CanRequest = true, ReasonCode = -1 };
		}

		public async Task<ICollection<ActiveCampaignsListForDeveloperDto>> GetActiveCampaignsListForDeveloper(string userId)
		{
			var query = await _context.Campaigns
				.Where(c => c.Game!.DeveloperId == userId && !c.IsClosed && (c.EndDate == null || c.EndDate > DateTime.Now))
				.Select(c => new ActiveCampaignsListForDeveloperDto
				{
					Id = c.Id,
					GameId = c.Game!.Id,
					GameName = c.Game!.Name,
					Keys = c.Keys.Select(rs => new RequestsSentAndKeysLeftDto
					{
						Id = rs.PlatformId,
						Name = rs.Platform!.Name,
						AcceptedRequests = c.Requests.Count(r => r.CampaignId == c.Id && r.PlatformId == rs.PlatformId && r.KeyId != null),
						KeysForCampaign = rs.IsUnlimited ? -1 : rs.MaximumNumberOfKeys,
						KeysLeft = c.Game.Keys.Count(k => k.PlatformId == rs.PlatformId && k.RequestId == null && k.GameId == c.GameId)
					}).ToList()
				}).ToListAsync();

			return query;
		}

		public async Task<CampaignStatsDto?> GetCampaignStats(int campaignId, string userId)
		{
			return await _context.Campaigns
				.Where(c => c.Id == campaignId && c.Game!.DeveloperId == userId)
				.Select(c => new CampaignStatsDto
				{
					Campaign = new CampaignDto
					{
						Id = c.Id,
						Game = new GameDto
						{
							Id = c.Game!.Id,
							Name = c.Game.Name,
							ReleaseDate = c.Game.ReleaseDate,
							Tags = c.Game.Tags.Select(tag => new TagDto
							{
								Id = tag.Id,
								Name = tag.Name
							}).ToList(),
							DeveloperId = c.Game.DeveloperId,
							CoverPhoto = c.Game.CoverPhoto ?? string.Empty
						},
						StartDate = c.StartDate,
						EndDate = c.EndDate,
						IsClosed = c.IsClosed
					},
					Keys = c.Keys.Select(k =>
					new RequestsSentAndKeysLeftDto
					{
						Id = k.PlatformId,
						Name = k.Platform!.Name,
						KeysForCampaign = k.IsUnlimited ? -1 : k.MaximumNumberOfKeys,
						AcceptedRequests = c.Requests.Count(r => r.CampaignId == c.Id && r.PlatformId == k.PlatformId && r.KeyId != null),
						KeysLeft = c.Game.Keys.Count(r => k.PlatformId == r.PlatformId && r.RequestId == null && r.GameId == c.GameId)
					}
					).ToList()
				})
				.FirstOrDefaultAsync();
		}
	}
}
