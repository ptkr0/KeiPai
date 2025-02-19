namespace Dtos.Account
{
	public class DeveloperFullInfoDto
	{
		public DeveloperDto Developer { get; set; } = new DeveloperDto();
		public int CampaignsCreated { get; set; }
		public int GamesAdded { get; set; }
	}
}
