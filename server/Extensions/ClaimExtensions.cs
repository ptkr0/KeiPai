using System.Security.Claims;

namespace Extensions
{
	public static class ClaimExtensions
	{
		public static string? GetEmail(this ClaimsPrincipal user)
		{
			return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
		}

		public static string? GetUserId(this ClaimsPrincipal user)
		{
			return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
		}

		public static string? GetRole(this ClaimsPrincipal user)
		{
			return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
		}

		public static string? GetUsername(this ClaimsPrincipal user)
		{
			return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
		}
	}
}
