using System.ComponentModel.DataAnnotations;

namespace Dtos.Campaign
{
	public class AssignKeysDto
	{
		[Required]
		public int PlatformId { get; set; }

		[Required]
		public int NumberOfKeys { get; set; }

		public bool IsUnlimited { get; set; } = false;
	}
}
