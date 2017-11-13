using Azure.Storage.SAS.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Azure.Storage.SAS.Controllers
{
	public class HomeController : Controller
	{
		CloudStorageAccount _storageAccount;
		CloudBlobClient _blobClient;
		CloudBlobContainer _container;

		public HomeController()
		{
			_storageAccount = CloudStorageAccount.Parse(
				CloudConfigurationManager.GetSetting("StorageConnection"));
			_blobClient = _storageAccount.CreateCloudBlobClient();
			_container = _blobClient.GetContainerReference("images");
		}

		// GET: Home
		public ActionResult Index()
		{
			var blobs = new List<BlobImage>();
			var sas = _container.GetSharedAccessSignature(null, "MySAP"); // GetSASToken();

			foreach (var blob in _container.ListBlobs())
			{
				if (blob.GetType() == typeof(CloudBlockBlob))
				{
					blobs.Add(new BlobImage() { BlobUri = String.Format("{0}{1}", blob.Uri.ToString(), sas) });
				}
			}

			return View(blobs);
		}

		private string GetSASToken()
		{
			SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
			{
				Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write,
				Services = SharedAccessAccountServices.Blob,
				ResourceTypes = SharedAccessAccountResourceTypes.Object,
				SharedAccessExpiryTime = DateTime.Now.AddMinutes(30),
				Protocols = SharedAccessProtocol.HttpsOnly
			};

			return _storageAccount.GetSharedAccessSignature(policy);
		}
	}
}