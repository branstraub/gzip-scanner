using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace gzip
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = new Options();
			// From where am I going to retrieve the paths
            options.ConnectionStringSource = args[0];
			// Where am I going to enqueue the paths
			options.ConnectionStringDestination = args[1];

			string containerSourceName = (args.Length > 2) ? args[2] : null;
            var storageAccountS = CloudStorageAccount.Parse(options.ConnectionStringSource);
			var storageAccountDestination = CloudStorageAccount.Parse(options.ConnectionStringDestination);

            var blobClientS = storageAccountS.CreateCloudBlobClient();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

			var util = new Utility(
				//new GzipBlober(storageAccountDestination)
				new GzipEnqueuer(storageAccountDestination)
			);

			if (string.IsNullOrEmpty(containerSourceName))
			{
				Console.WriteLine("All containers");
				foreach (var container in blobClientS.ListContainers())
				{
					var blobContainerS = blobClientS.GetContainerReference(container.Name);
					Console.WriteLine($"Starting container {container.Name}");
					await util.EnsureGzipFiles(blobContainerS);
					Console.WriteLine($"Finished container {container.Name}");
				}
			}
            else
			{
				Console.WriteLine("Specific container");
				var container = blobClientS.GetContainerReference(containerSourceName);
				Console.WriteLine($"Starting container {container.Name}");
				await util.EnsureGzipFiles(container);
				Console.WriteLine($"Finished container {container.Name}");
			}

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            var ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
           
            Console.WriteLine("RunTime " + ts);
        }
    }
}
    