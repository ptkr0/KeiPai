using Data;
using Models;

namespace Initialization
{
	public class TagInitialization
	{
		public static async Task InitializeTags(IServiceProvider serviceProvider)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

				if (!context.Tags.Any())
				{
					context.Tags.AddRange(
						new Tag { Name = "Action" },
						new Tag { Name = "Adventure" },
						new Tag { Name = "Casual" },
						new Tag { Name = "Indie" },
						new Tag { Name = "MMO" },
						new Tag { Name = "Open World" },
						new Tag { Name = "Puzzle" },
						new Tag { Name = "Shooter" },
						new Tag { Name = "Singleplayer" },
						new Tag { Name = "Multiplayer" },
						new Tag { Name = "Racing" },
						new Tag { Name = "RPG" },
						new Tag { Name = "Simulation" },
						new Tag { Name = "Sports" },
						new Tag { Name = "Strategy" }

					);

					await context.SaveChangesAsync();
				}
			}
		}
	}
}
