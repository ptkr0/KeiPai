using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Keys")]
	public class Key
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Value { get; set; } = string.Empty;

		public int GameId { get; set; } // foreign key to Game table
		public Game? Game { get; set; }

		public int? PlatformId { get; set; } // foreign key to Platform table
		public Platform? Platform { get; set; }

		public int? RequestId { get; set; } // foreign key to Request table
		public Request? Request { get; set; }
	}
}
