using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace gzip
{
	public class GzipEnqueuer : IActor
	{
		private readonly string queueName = "gzip";
		private readonly CloudStorageAccount account;

		public GzipEnqueuer(CloudStorageAccount account)
		{
			this.account = account ?? throw new ArgumentNullException(nameof(account));
			var queueClient = account.CreateCloudQueueClient();
			var queueRef = queueClient.GetQueueReference(queueName);
			queueRef.CreateIfNotExistsAsync().Wait();
		}

		public async Task Act(string content)
		{
			var queueClient = account.CreateCloudQueueClient();
			var queue = queueClient.GetQueueReference("gzip");
			var message = new CloudQueueMessage(content);
			await queue.AddMessageAsync(message);
		}
	}
}
