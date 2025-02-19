using System.ComponentModel.DataAnnotations;

namespace Dtos.Key
{
	public class KeyInCollectionDto
	{
		[MaxLength(50)]
		public string Key { get; set; } = string.Empty;
	}
}
