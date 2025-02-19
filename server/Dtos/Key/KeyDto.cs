namespace Dtos.Key
{
	public class KeyDto
	{
		public int Id { get; set; }
		public string Value { get; set; }
		public int? GameId { get; set; }
		public int? PlatformId { get; set; }
		public string? PlatformName { get; set; }
	}
}
