namespace Dtos.OtherMedia
{
	public class AddOtherMedia
	{
		public required string Name { get; set; }
		public required string Url { get; set; }
		public string Role { get; set; } = string.Empty;
		public string Medium { get; set; } = string.Empty;
		public string SampleContent { get; set; } = string.Empty;
	}
}
