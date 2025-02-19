using Dtos.Account;
using Extensions;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers
{
	[Route("api/account")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IDeveloperRepository _developerService;
		private readonly UserManager<User> _userManager;
		private readonly IInfluencerRepository _influencerService;

		public UserController(IUserService userService, IDeveloperRepository developerService, UserManager<User> userManager, IInfluencerRepository influencerService)
		{
			_userService = userService;
			_developerService = developerService;
			_userManager = userManager;
			_influencerService = influencerService;
		}

		[HttpPost("developer/register")]
		[SwaggerOperation(Summary = "endpoint to register a developer")]
		public async Task<IActionResult> Register([FromBody] DeveloperRegisterDto register)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (createResult, user) = await _userService.CreateDeveloperAsync(register);
			if (!createResult.Succeeded)
				return BadRequest(createResult.Errors);

			var roleResult = await _userService.AddRoleAsync(user, "Developer");
			if (!roleResult.Succeeded)
				return BadRequest(roleResult.Errors);

			var added = await _developerService.AddDeveloperInfoAsync(user, register.WebsiteUrl);
			if (!added)
				return BadRequest("Failed to create developer profile");

			var token = _userService.CreateToken(user, "Developer");

			return Ok(new NewUserDto
			{
				Id = user.Id,
				Username = user.UserName,
				Email = user.Email,
				Role = "Developer",
				Token = token
			});
		}

		[HttpPost("influencer/register")]
		[SwaggerOperation(Summary = "endpoint to register an influencer")]
		public async Task<IActionResult> Register([FromBody] InfluencerRegisterDto register)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (createResult, user) = await _userService.CreateInfluencerAsync(register);
			if (!createResult.Succeeded)
				return BadRequest(createResult.Errors);

			var roleResult = await _userService.AddRoleAsync(user, "Influencer");
			if (!roleResult.Succeeded)
				return BadRequest(roleResult.Errors);

			var language = await _influencerService.AddInfluencerInfo(user, register.Language);
			if (!language)
				return BadRequest("Failed to create influencer profile");
			var token = _userService.CreateToken(user, "Influencer");
			return Ok(new NewUserDto
			{
				Id = user.Id,
				Username = user.UserName,
				Email = user.Email,
				Role = "Influencer",
				Token = token
			}
			);

		}

		[HttpPost("login")]
		[SwaggerOperation(Summary = "endpoint to login a user (works for both developer and influencer)")]
		public async Task<IActionResult> Login([FromBody] LoginDto login)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (loginResult, user) = await _userService.ValidateUserAsync(login.Email, login.Password);
			if (user == null || !loginResult.Succeeded)
				return Unauthorized("Invalid password or email");

			var role = await _userService.GetRoleAsync(user);
			var token = _userService.CreateToken(user, role[0]);

			Response.Cookies.Append("Authorization", token, new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.None,
				Secure = true
			});

			return Ok(
				new NewUserDto
				{
					Id = user.Id,
					Username = user.UserName,
					Email = user.Email,
					Role = role[0],
					Token = token
				}
			);


		}

		[HttpPost("logout")]
		[SwaggerOperation(Summary = "endpoint to logout a user")]
		public async Task<IActionResult> Logout()
		{
			Response.Cookies.Delete("Authorization", new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.None,
				Secure = true
			});
			await Task.Delay(0);

			return Ok("{}");
		}

		[HttpPatch("d")]
		[Authorize(Roles = "Developer")]
		[SwaggerOperation(Summary = "endpoint to update developer info")]
		public async Task<IActionResult> UpdateDeveloper([FromBody] UpdateDeveloperDto developerDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null)
				return Unauthorized();

			if (developerDto.Username != null)
			{
				var username = await _userManager.FindByNameAsync(developerDto.Username);
				if (username != null)
					return Unauthorized();
			}

			var result = await _developerService.UpdateDeveloper(userId, developerDto);

			if (result == null)
				return NotFound();

			if (developerDto.Username != null)
			{
				var user = await _userManager.FindByIdAsync(userId);
				var token = _userService.CreateToken(user!, "Developer");

				Response.Cookies.Append("Authorization", token, new CookieOptions
				{
					HttpOnly = true,
					SameSite = SameSiteMode.None,
					Secure = true
				});
			}

			return Ok(result);
		}

		[HttpPatch("i")]
		[Authorize(Roles = "Influencer")]
		[SwaggerOperation(Summary = "endpoint to update influencer info")]
		public async Task<IActionResult> UpdateInfluencer([FromBody] UpdateInfluecnerDto influecnerDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null)
				return Unauthorized();

			if (influecnerDto.Username != null)
			{
				var username = await _userManager.FindByNameAsync(influecnerDto.Username);
				if (username != null)
					return Unauthorized();
			}

			var result = await _influencerService.UpdateInfluencer(userId, influecnerDto);

			if (result == null)
				return NotFound();

			if (influecnerDto.Username != null)
			{
				var user = await _userManager.FindByIdAsync(userId);
				var token = _userService.CreateToken(user!, "Influencer");

				Response.Cookies.Append("Authorization", token, new CookieOptions
				{
					HttpOnly = true,
					SameSite = SameSiteMode.None,
					Secure = true
				});
			}

			return Ok(result);
		}

		[HttpGet("info")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get very basic user info (used in frontend for guards)")]
		public IActionResult GetUserInfo()
		{
			var id = User.GetUserId();
			if (id == null) return Unauthorized();

			var username = User.GetUsername();

			var role = User.GetRole();
			if (role == null) return Unauthorized();

			var email = User.GetEmail();
			if (email == null) return Unauthorized();

			return Ok(new
			{
				id,
				username,
				role,
				email
			});
		}

		[HttpGet("influencer/info")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get authorized influencer info")]
		public async Task<IActionResult> GetInfluencerInfo()
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var role = User.GetRole();
			if (role == null) return Unauthorized();

			var influencerInfo = await _influencerService.GetInfluencerInfo(userId);

			return Ok(influencerInfo);
		}

		[HttpGet("find/{username}")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get full info of an influencer or developer")]
		public async Task<IActionResult> GetInfluencerFullInfo([FromRoute] string username)
		{
			var user = await _userManager.FindByNameAsync(username);
			if (user == null) return NotFound();

			var role = await _userManager.GetRolesAsync(user);
			if (role[0] == "Influencer")
			{
				var influencerInfo = await _influencerService.GetInfluencerFullInfo(user.Id);
				return Ok(new { influencer = influencerInfo });
			}

			else if (role[0] == "Developer")
			{
				var developerInfo = await _developerService.GetDeveloperFullInfo(user.Id);
				return Ok(new { developer = developerInfo });
			}

			else return BadRequest("User not found");
		}
	}
}
