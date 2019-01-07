using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Queue;

namespace gzip
{
    class Utility
    {
		private string DestinationConnectionString { get; }
		private readonly IActor someone;

		public Utility(IActor someone)
		{
			this.someone = someone ?? throw new ArgumentNullException(nameof(someone));
		}

        public async Task EnsureGzipFiles(CloudBlobContainer containerS)
        {
			//segmented and await
			//var blobInfos = containerS.ListBlobs("", true, BlobListingDetails.Metadata);
			List<IListBlobItem> blobInfos = new List<IListBlobItem>();
			BlobContinuationToken continuationToken = null;
			var iteration = 0;
			do
			{
				var response = await containerS.ListBlobsSegmentedAsync("", true, BlobListingDetails.Metadata, null, continuationToken, null, null);
				Console.WriteLine($"Iteration: {++iteration} with # of blobs received for the request {containerS.Name}: {response.Results.Count()}");
				continuationToken = response.ContinuationToken;
				blobInfos.AddRange(response.Results);

				foreach(var blob in blobInfos){
					await someone.Act(blob.Uri.ToString());
            	}	
				
				Console.WriteLine($"# {blobInfos.Count} of blobs added to the queue");
				blobInfos.Clear();
			}
			while (continuationToken != null);
		}
    }
}
