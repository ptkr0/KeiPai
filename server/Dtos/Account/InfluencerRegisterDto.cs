using System.ComponentModel.DataAnnotations;

namespace Dtos.Account
{
	public class InfluencerRegisterDto
	{
		[Required]
		public string? Username { get; set; }

		[Required]
		[EmailAddress]
		public string? Email { get; set; }

		[Required]
		[MinLength(8)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")]
		public string? Password { get; set; }

		[MaxLength(200)]
		public string? About { get; set; } = string.Empty;

		[EmailAddress]
		[MaxLength(32)]
		public string? ContactEmail { get; set; } = string.Empty;

		[Required]
		public string? Language { get; set; } = string.Empty;
	}
}
