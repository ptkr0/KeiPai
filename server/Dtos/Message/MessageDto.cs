namespace Dtos.Message
{
	public class MessageDto
	{
		public int Id { get; set; }
		public string SenderId { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}
