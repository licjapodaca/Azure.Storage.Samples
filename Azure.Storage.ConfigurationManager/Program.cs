using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Storage.ConfigurationManager
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				#region Creating the container in Azure

				var storageAccount = CloudStorageAccount.Parse(
					CloudConfigurationManager.GetSetting("StorageConnection"));

				var blobClient = storageAccount.CreateCloudBlobClient();

				var container = blobClient.GetContainerReference("images");

				container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

				#endregion

				#region Uploading files to the container

				//var blockBlob = container.GetBlockBlobReference("image.png");

				//using (var fileStream = System.IO.File.OpenRead(@"C:\Users\japodaca\OneDrive\MCSD\Pictures\jorge-apodaca.png"))
				//{
				//	blockBlob.UploadFromStream(fileStream);
				//}

				#endregion

				#region List files in a container

				//var blobs = container.ListBlobs();

				//foreach (var file in blobs)
				//{
				//	Console.WriteLine(file.Uri);
				//}

				#endregion

				#region Download files from the container

				//var blockBlob = container.GetBlockBlobReference("image.png");

				//using (var fileStream = System.IO.File.OpenWrite(@"C:\temp\jorge-apodaca.png"))
				//{
				//	blockBlob.DownloadToStream(fileStream);
				//}

				#endregion

				#region Implementing Async Blob Copy

				//var blockBlob = container.GetBlockBlobReference("image.png");
				//var blockBlobCopy = container.GetBlockBlobReference("image2.png");

				//var cb = new AsyncCallback(x => Console.WriteLine("blob copy completed"));

				//blockBlobCopy.BeginStartCopy(blockBlob.Uri, cb, null);

				#endregion

				#region Hierarchy folders

				//var blockBlob = container.GetBlockBlobReference("new-folder/image4.png");

				//using (var fileStream = System.IO.File.OpenRead(@"C:\Users\japodaca\OneDrive\MCSD\Pictures\jorge-apodaca.png"))
				//{
				//	blockBlob.UploadFromStream(fileStream);
				//}

				#endregion

				#region Setting metadata to a container

				//SetMetaData(container);

				#endregion

				#region Reading metadata from the container

				//GetMetaData(container);

				#endregion

				Console.ReadKey();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				Console.ReadKey();
			}
		}

		static void GetMetaData(CloudBlobContainer container)
		{
			container.FetchAttributes();
			Console.WriteLine("Container Metadata: \n");
			foreach (var item in container.Metadata)
			{
				Console.WriteLine(
					String.Format("{0}: {1}", item.Key, item.Value));
			}
		}

		static void SetMetaData(CloudBlobContainer container)
		{
			container.Metadata.Clear();
			container.Metadata.Add("Owner", "Jorge");
			container.Metadata["Updated"] = DateTime.Now.ToString();
			container.SetMetadata();
		}
	}
}
