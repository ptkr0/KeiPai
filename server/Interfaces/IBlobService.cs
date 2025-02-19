using Models;

namespace Interfaces
{
	public interface IBlobService
	{
		Task<BlobInfo> GetBlobAsync(string name); // not used
		Task<IEnumerable<string>> ListBlobsAsync(); // not implemented
		Task<string> UploadFileBlobAsync(IFormFile file);
		Task UploadContentBlobAsync(string content, string name); // not used
		Task DeleteBlobAsync(string name);
		Task<string> GetBlobStringAsync(string name);
	}
}
