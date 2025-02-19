using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Interfaces;
using System.Text;

namespace Services
{
	public class BlobService : IBlobService
	{
		private readonly BlobServiceClient _blobServiceClient;
		public BlobService(BlobServiceClient blobServiceClient)
		{
			_blobServiceClient = blobServiceClient;
		}

		/// <summary>
		/// deletes an image from the azure blob storage
		/// </summary>
		/// <param name="url">url to an image</param>
		/// <returns></returns>
		public async Task DeleteBlobAsync(string url)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("images");

			// to delete an image we need to get the name of the image (last part of the url)
			var name = url.Split("/").Last();
			var blobClient = containerClient.GetBlobClient(name);

			await blobClient.DeleteIfExistsAsync();
		}

		public async Task<Models.BlobInfo> GetBlobAsync(string name)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("images");
			var blobClient = containerClient.GetBlobClient(name);
			var blobDownloadInfo = await blobClient.DownloadAsync();

			return new Models.BlobInfo
			{
				Content = blobDownloadInfo.Value.Content,
				ContentType = blobDownloadInfo.Value.ContentType
			};
		}

		/// <summary>
		/// returns an url to the image on azure blob storage
		/// </summary>
		/// <param name="name">name of the image</param>
		/// <returns>an url to the image</returns>
		public async Task<string> GetBlobStringAsync(string name)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("images");
			var blobClient = containerClient.GetBlobClient(name);
			await Task.Delay(0);
			return blobClient.Uri.ToString();
		}

		public Task<IEnumerable<string>> ListBlobsAsync()
		{
			throw new NotImplementedException();
		}

		public async Task UploadContentBlobAsync(string content, string name)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient("images");
			var blobClient = containerClient.GetBlobClient(name);
			var bytes = Encoding.UTF8.GetBytes(content);
			using var memoryStream = new MemoryStream(bytes);
			await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = "text/plain" });
		}

		/// <summary>
		/// uploads a file to the azure blob storage
		/// </summary>
		/// <param name="file"></param>
		/// <returns>url to the uploaded file</returns>
		public async Task<string> UploadFileBlobAsync(IFormFile file)
		{
			var fileName = Guid.NewGuid().ToString() + file.FileName;
			var containerClient = _blobServiceClient.GetBlobContainerClient("images");
			var blobClient = containerClient.GetBlobClient(fileName);

			using (var stream = file.OpenReadStream())
			{
				await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
			}

			return blobClient.Uri.ToString();
		}
	}
}
