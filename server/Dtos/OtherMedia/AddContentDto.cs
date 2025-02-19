namespace Dtos.OtherMedia
{
	public class AddContentDto
	{
		public required string Title { get; set; }
		public string Description { get; set; } = string.Empty;
		public required string Url { get; set; }
		public ICollection<int>? GameIds { get; set; }
		public IFormFile? Thumbnail { get; set; }
	}
}
