using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace gzip
{
	public class GzipBlober : IActor
	{
		private readonly CloudStorageAccount account;
		private readonly string containerName = "gzip";

		public GzipBlober(CloudStorageAccount account)
		{
			this.account = account ?? throw new ArgumentNullException(nameof(account));
			var blobClient = account.CreateCloudBlobClient();
			var blobContainer = blobClient.GetContainerReference(containerName);
			blobContainer.CreateIfNotExists();
		}

		public async Task Act(string content)
		{
			var blobName = String.Copy(content);
			blobName = Regex.Replace(blobName, @"\s+", "");
			blobName = Regex.Replace(blobName, "/", "");
			blobName = $"{blobName}-{Guid.NewGuid()}";
			var blobClient = account.CreateCloudBlobClient();
			var blobContainer = blobClient.GetContainerReference(containerName);
			CloudBlockBlob cloudBlockBlob = blobContainer.GetBlockBlobReference(blobName);
			await cloudBlockBlob.UploadTextAsync(content);
		}
	}
}
