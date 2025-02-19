namespace Dtos.Key
{
	public class DeleteKeysDto
	{
		public ICollection<int> KeyIds { get; set; } = new List<int>();
	}
}
