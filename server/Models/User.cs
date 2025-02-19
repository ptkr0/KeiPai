using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Models
{
	public class User : IdentityUser
	{
		[MaxLength(200)]
		public string About { get; set; } = string.Empty;

		[MaxLength(32)]
		[EmailAddress]
		public string? ContactEmail { get; set; } = string.Empty;

		public string? ProfilePicture { get; set; } = string.Empty;

		// user can be a developer or an influencer
		public Developer? Developer { get; set; }
		public Influencer? Influencer { get; set; }

		// user can have multiple messages sent and received
		public ICollection<Message> SentMessages { get; set; } = new List<Message>();
		public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
	}
}
