using Microsoft.AspNetCore.Identity;
using Models;

namespace Interfaces
{
	public interface ITokenService
	{
		string CreateToken(User user, string role);
	}
}
