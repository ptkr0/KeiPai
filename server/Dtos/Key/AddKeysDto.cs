using System.ComponentModel.DataAnnotations;

namespace Dtos.Key
{
	public class AddKeysDto
	{
		[Required]
		public int GameId { get; set; }

		[Required]
		public int PlatformId { get; set; }

		[Required]
		public ICollection<KeyInCollectionDto> Keys { get; set; } = new List<KeyInCollectionDto>();
	}
}
