namespace Dtos.Youtube
{
	public class ExchangedCodeForTokensDto
	{
		public required string AccessToken { get; set; }
		public required string RefreshToken { get; set; }
	}
}
