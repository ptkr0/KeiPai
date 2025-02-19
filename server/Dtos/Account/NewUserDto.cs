using System.Globalization;

namespace Dtos.Account
{
	public class NewUserDto
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
		public string Token { get; set; }
	}
}
