using System.ComponentModel.DataAnnotations;

namespace Dtos.Account
{
	public class DeveloperRegisterDto
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

		[MaxLength(50)]
		public string WebsiteUrl { get; set; } = string.Empty;
	}
}
