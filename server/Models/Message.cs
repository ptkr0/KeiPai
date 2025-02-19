using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Messages")]
	public class Message
	{
		[Key]
		public int Id { get; set; }
		public string SenderId { get; set; } = string.Empty;
		public User? Sender { get; set; }

		public string ReceiverId { get; set; } = string.Empty;
		public User? Receiver { get; set; }

		public string Content { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}
