using Data;
using Dtos.Account;
using Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<User> _userManager;
		private readonly ITokenService _tokenService;
		private readonly SignInManager<User> _signInManager;
		private readonly IDeveloperRepository _developerService;
		private readonly IInfluencerRepository _influencerService;
		private readonly ApplicationDBContext _context;

		public UserService(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, IDeveloperRepository developerService, IInfluencerRepository influencerService, ApplicationDBContext applicationDbContext)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_signInManager = signInManager;
			_developerService = developerService;
			_influencerService = influencerService;
			_context = applicationDbContext;
		}

		public async Task<(IdentityResult, User)> CreateDeveloperAsync(DeveloperRegisterDto registerDto)
		{
			var user = new User
			{
				UserName = registerDto.Username,
				Email = registerDto.Email,
				ContactEmail = registerDto.ContactEmail,
				About = registerDto.About ?? string.Empty,
			};

			var result = await _userManager.CreateAsync(user, registerDto.Password);
			return (result, user);
		}
		public async Task<(IdentityResult, User)> CreateInfluencerAsync(InfluencerRegisterDto registerDto)
		{
			var user = new User
			{
				UserName = registerDto.Username,
				Email = registerDto.Email,
				ContactEmail = registerDto.ContactEmail,
				About = registerDto.About ?? string.Empty,
			};

			var result = await _userManager.CreateAsync(user, registerDto.Password);
			return (result, user);
		}
		public async Task<IdentityResult> AddRoleAsync(User user, string role)
		{
			return await _userManager.AddToRoleAsync(user, role);
		}

		public string CreateToken(User user, string role)
		{
			return _tokenService.CreateToken(user, role);
		}

		public async Task<(SignInResult, User?)> ValidateUserAsync(string email, string password)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return (SignInResult.Failed, null);

			var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
			return (result, user);
		}

		public async Task<IList<string>> GetRoleAsync(User user)
		{
			return await _userManager.GetRolesAsync(user);
		}

		public async Task<DeveloperDto> GetDeveloper(User user)
		{
			var additionalInfo = await _developerService.GetDeveloperInfoAsync(user);
			return new DeveloperDto
			{
				Id = user.Id,
				Username = user.UserName ?? string.Empty,
				Email = user.Email ?? string.Empty,
				About = user.About,
				ContactEmail = user.ContactEmail ?? string.Empty,
				WebsiteUrl = additionalInfo?.WebsiteUrl ?? string.Empty
			};

		}

		public async Task<InfluencerDto> GetInfluencer(User user)
		{
			var additionalInfo = await _influencerService.GetInfluencerInfoAsync(user);
			return new InfluencerDto
			{
				Id = user.Id,
				Username = user.UserName ?? string.Empty,
				Email = user.Email ?? string.Empty,
				About = user.About,
				ContactEmail = user.ContactEmail ?? string.Empty,
				Language = additionalInfo?.Language ?? string.Empty
			};
		}

		public async Task<BasicUserInfoDto?> GetBasicUserInfo(string userId)
		{
			return await _context.Users
				.Where(u => u.Id == userId)
				.Select(u => new BasicUserInfoDto
				{
					UserId = u.Id,
					Username = u.UserName ?? string.Empty,
					ContactEmail = u.ContactEmail ?? string.Empty,
					About = u.About
				})
				.FirstOrDefaultAsync();
		}
	}
}
