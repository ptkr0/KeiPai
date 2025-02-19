namespace Dtos.Youtube
{
	public class YoutubeVideosPaginated
	{
		public ICollection<GetYoutubeVideoDto> Videos { get; set; } = new List<GetYoutubeVideoDto>();
		public int CurrentPage { get; set; }
		public int PageSize { get; set; }
		public int TotalPages { get; set; }
		public int TotalCount { get; set; }
	}
}
