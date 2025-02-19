namespace Dtos.Game
{
	public class PaginatedGames
	{
		public ICollection<GameDto> Games { get; set; } = new List<GameDto>();
		public int TotalCount { get; set; }
		public int TotalPages { get; set; } = 0;
		public int CurrentPage { get; set; } = 1;
		public int PageSize { get; set; }
	}
}
