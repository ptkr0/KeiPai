using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Tags")]
	public class Tag
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public ICollection<Game> Games { get; set; } = new List<Game>();
	}
}
