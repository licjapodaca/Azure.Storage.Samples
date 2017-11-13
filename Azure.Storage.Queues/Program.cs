using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Storage.Queues
{
	class Program
	{
		static void Main(string[] args)
		{
			#region Creating the Storage Queue

			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
					CloudConfigurationManager.GetSetting("StorageConnection"));

			CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

			CloudQueue queue = queueClient.GetQueueReference("tasks");

			queue.CreateIfNotExists();

			#endregion

			#region Create queue Message

			//CloudQueueMessage message = new CloudQueueMessage("Hello World!");
			//var time = new TimeSpan(24, 0, 0);
			//queue.AddMessage(message, time, null, null);

			#endregion

			#region  Get the Message from the Queue Storage

			//CloudQueueMessage message = queue.GetMessage();
			//Console.WriteLine("{0}", message.AsString);
			//queue.DeleteMessage(message);

			//CloudQueueMessage message = queue.PeekMessage();
			//Console.WriteLine("{0}", message.AsString);

			#endregion

			Console.ReadKey();
		}
	}
}
