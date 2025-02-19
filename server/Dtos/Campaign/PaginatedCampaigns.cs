namespace Dtos.Campaign
{
	public class PaginatedCampaigns
	{
		public ICollection<CampaignDto> Campaigns { get; set; } = new List<CampaignDto>();
		public int TotalCount { get; set; }
		public int TotalPages { get; set; } = 0;
		public int CurrentPage { get; set; } = 1;
		public int PageSize { get; set; }
	}
}
