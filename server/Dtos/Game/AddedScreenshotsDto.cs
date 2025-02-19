namespace Dtos.Game
{
	public class AddedScreenshotsDto
	{

		public ICollection<ScreenshotDto> screenshots { get; set; } = new List<ScreenshotDto>();
	}
}
