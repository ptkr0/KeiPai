using IGDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers
{
	[Route("api/igdb")]
	public class IGDBController : ControllerBase
	{
		private readonly IConfiguration _config;

		public IGDBController(IConfiguration config)
		{
			_config = config;
		}

		[HttpGet("{keyword}")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to search games in the IGDB database")]
		public async Task<IActionResult> GetGames([FromRoute] string keyword)
		{
			string clientId = _config["Authentication:Twitch:ClientId"] ?? throw new InvalidOperationException("Twitch ClientId is required.");
			string secret = _config["Authentication:Twitch:ClientSecret"] ?? throw new InvalidOperationException("Twitch Client Secret is required.");
			var igdb = new IGDBClient(clientId, secret);

			var games = await igdb.QueryAsync<IGDB.Models.Game>(IGDBClient.Endpoints.Games, $"fields id, name; search \"{keyword}\";");

			var gameDtos = games.Select(g => new
			{
				g.Id,
				g.Name,
			}).ToList();

			return Ok(gameDtos);
		}
	}
}
