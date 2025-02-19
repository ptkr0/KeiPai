using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Initialization
{
	public class UserInitialization
	{
		private static readonly (string email, string password, string role, string about)[] users = {
		("admin@example.com", "Admin123!", "Administrator", "I'm an admin"),
		("moderator@example.com", "Moderator123!", "Moderator", "I'm a moderator"),
		// ("developer@example.com", "Developer123!", "Developer", "I'm a developer"),
		// ("influencer@example.com", "Influencer123!", "Influencer", "I'm an indluencer")
		};

		public static async Task InitializeUsers(IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

			foreach (var user in users)
			{
				if (await userManager.FindByEmailAsync(user.email) == null)
				{
					var newUser = new User { UserName = user.email, Email = user.email, ContactEmail = user.email, About = user.about };
					var result = await userManager.CreateAsync(newUser, user.password);
					if (result.Succeeded)
					{
						await userManager.AddToRoleAsync(newUser, user.role);
					}
				}
			}
		}
	}
}

