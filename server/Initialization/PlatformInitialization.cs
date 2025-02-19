using Data;
using Models;

namespace Initialization
{
	public class PlatformInitialization
	{
		public static async Task InitializePlatforms(IServiceProvider serviceProvider)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

				if (!context.Platforms.Any())
				{
					context.Platforms.AddRange(
						new Platform { Name = "Steam" },
						new Platform { Name = "Epic Games" },
						new Platform { Name = "Origin" },
						new Platform { Name = "Uplay" },
						new Platform { Name = "Battle.net" },
						new Platform { Name = "GOG" },
						new Platform { Name = "itch.io" },
						new Platform { Name = "Xbox One" },
						new Platform { Name = "Xbox Series X" },
						new Platform { Name = "PlayStation 5" },
						new Platform { Name = "PlayStation 4" },
						new Platform { Name = "Nintendo Switch" },
						new Platform { Name = "Android" },
						new Platform { Name = "iOS" },
						new Platform { Name = "Other" }
					);

					await context.SaveChangesAsync();
				}
			}
		}
	}
}
