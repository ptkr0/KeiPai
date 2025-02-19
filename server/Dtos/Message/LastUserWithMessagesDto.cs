namespace Dtos.Message
{
	public class LastUserWithMessagesDto
	{
		public string UserId { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string LastMessage { get; set; } = string.Empty;
		public DateTime LastMessageDate { get; set; }
	}
}
