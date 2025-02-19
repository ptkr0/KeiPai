using Dtos.Account;
using Microsoft.AspNetCore.Identity;
using Models;

namespace Interfaces
{
	public interface IUserService
	{
		Task<(IdentityResult, User)> CreateDeveloperAsync(DeveloperRegisterDto registerDto);
		Task<(IdentityResult, User)> CreateInfluencerAsync(InfluencerRegisterDto registerDto);
		Task<IdentityResult> AddRoleAsync(User user, string role);
		string CreateToken(User user, string role);
		Task<(SignInResult, User?)> ValidateUserAsync(string email, string password);
		Task<IList<string>> GetRoleAsync(User user);
		Task<DeveloperDto> GetDeveloper(User user);
		Task<InfluencerDto> GetInfluencer(User user);
		Task<BasicUserInfoDto?> GetBasicUserInfo(string userId);
	}
}
