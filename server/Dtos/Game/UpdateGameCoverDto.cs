using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Dtos.Game
{
	public class UpdateGameCoverDto
	{
		[Required]
		[SwaggerSchema(Description = "file needs to be in .jpg, .jpeg, .png or .webp. max file size can be 1mb")]
		public required IFormFile CoverPhoto { get; set; }
	}
}
