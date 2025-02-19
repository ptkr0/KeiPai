namespace Dtos.Twitch
{
	public class TwitchStreamsPaginated
	{
		public ICollection<GetTwitchStreamDto> Streams { get; set; } = new List<GetTwitchStreamDto>();
		public int CurrentPage { get; set; }
		public int PageSize { get; set; }
		public int TotalPages { get; set; }
		public int TotalCount { get; set; }
	}
}
