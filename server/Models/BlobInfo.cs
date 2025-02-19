namespace Models
{
	public class BlobInfo
	{
		public required Stream Content { get; set; }
		public required string ContentType { get; set; }
	}
}
