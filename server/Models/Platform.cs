using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
	[Table("Platforms")]
	public class Platform
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; } = string.Empty;

		public ICollection<Key> Keys { get; set; } = new List<Key>(); // list of keys for this platform
	}
}
