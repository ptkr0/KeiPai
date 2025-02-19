using Dtos.Key;
using Extensions;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers;

[Route("api/key")]
[ApiController]
public class KeyController : ControllerBase
{
	private readonly IGameRepository _gameRepo;
	private readonly IKeyRepository _keyRepo;
	private readonly UserManager<User> _userManager;

	public KeyController(UserManager<User> userManager, IGameRepository gameRepo, IKeyRepository keyRepo)
	{
		_userManager = userManager;
		_gameRepo = gameRepo;
		_keyRepo = keyRepo;
	}

	[HttpPost]
	[Authorize(Roles = "Developer")]
	[SwaggerOperation(Summary = "endpoint to add keys to a game")]
	public async Task<IActionResult> AddKeys([FromBody] AddKeysDto addKeys)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var userId = User.GetUserId();
		if (userId == null) return Unauthorized();

		var game = await _gameRepo.GetById(addKeys.GameId, userId);
		if (game == null) return NotFound("Invalid Game");

		var result = await _keyRepo.AddKeys(addKeys);

		return Ok(result);
	}

	[HttpDelete]
	[Authorize(Roles = "Developer")]
	[SwaggerOperation(Summary = "endpoint to delete keys")]
	public async Task<IActionResult> DeleteKeys([FromBody] DeleteKeysDto deleteKeys)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var userId = User.GetUserId();
		if (userId == null) return Unauthorized();

		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return Unauthorized();

		var result = await _keyRepo.DeleteKeys(deleteKeys, user);

		return Ok(result);
	}

	[HttpGet("{id}")]
	[Authorize(Roles = "Developer")]
	[SwaggerOperation(Summary = "endpoint to get keys by game")]
	public async Task<IActionResult> GetKeysByGame([FromRoute] int id, [FromQuery] List<int>? platforms, int pageNumber = 1)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var userId = User.GetUserId();
		if (userId == null) return Unauthorized();

		var pageSize = 100;

		var game = await _gameRepo.CheckIfUserIsOwner(id, userId);
		if (game == null) return Unauthorized("Can't access this resource");

		var result = await _keyRepo.GetByGameId(id, pageNumber, pageSize, platforms);

		return Ok(result);
	}
}