namespace Dtos.Account
{
	public class UpdateInfluecnerDto
	{
		public string? Username { get; set; }
		public string? ContactEmail { get; set; }
		public string? About { get; set; }
		public string Language { get; set; } = string.Empty;
	}
}
