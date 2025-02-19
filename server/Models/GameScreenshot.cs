using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("GameScreenshots")]
	public class GameScreenshot
	{
		[Key]
		public int Id { get; set; }
		public string Image { get; set; } = string.Empty;
		public int? GameId { get; set; }
		public Game? Game { get; set; }
	}
}
