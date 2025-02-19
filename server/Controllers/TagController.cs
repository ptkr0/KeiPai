using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers
{
	[Route("api/tag")]
	public class TagController : ControllerBase
	{
		private readonly ITagRepository _tag;
		public TagController(ITagRepository tag)
		{
			_tag = tag;
		}

		[HttpGet]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get all tags for games")]
		public async Task<IActionResult> GetTags()
		{
			var tags = await _tag.GetAll();
			var tagsDto = tags.Select(t => new { t.Id, t.Name }).ToList();

			return Ok(tagsDto);
		}
	}
}
