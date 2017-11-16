using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.File.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Storage.Files
{
	class Program
	{
		static CloudStorageAccount _storageAccount;
		static CloudFileShare _share;
		static ConsoleKeyInfo _opcion;

		static Program()
		{
			try
			{
				CreateShareFile();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		static void Main(string[] args)
		{
			string path = "", pathRemote = "";
			bool OPCION_EXISTE = true;

			while (_opcion.KeyChar.ToString().Trim().ToUpper() != "X")
			{
				Console.Clear();
				Console.WriteLine("(1) Listing files and directories from a path");
				Console.WriteLine("(2) Upload file");
				Console.WriteLine("(3) Download file");
				Console.WriteLine("(4) Delete file");
				Console.WriteLine("(X) Exit\n\n");
				Console.Write("Opcion: ");

				_opcion = Console.ReadKey();

				switch (_opcion.KeyChar.ToString().Trim().ToUpper())
				{
					case "1":

						Console.Clear();
						Console.Write("\n\nType '/' if you want the root directory, otherwise type the name of the hierarchy directories...\n\nPath: ");

						path = Console.ReadLine();

						GetFilesAndDirectories(path);

						break;

					case "2":

						Console.Clear();
						Console.WriteLine("\n\nType the path name and file you want to upload...");

						Console.Write("\nLocal Path: ");
						path = Console.ReadLine();
						Console.Write("\nRemote Path: ");
						pathRemote = Console.ReadLine();

						if (String.IsNullOrEmpty(path)) FileUpload(); else FileUpload(path, pathRemote);

						break;

					case "3":

						Console.Clear();
						Console.WriteLine("\n\nType the path name and file you want to download...");
						Console.WriteLine("\nPath: ");

						path = Console.ReadLine();

						if (String.IsNullOrEmpty(path)) FileDownload(); else FileDownload(path);

						break;

					case "4":

						Console.Clear();
						Console.WriteLine("\n\nType the path name and file you want to download...");
						Console.WriteLine("\nPath: ");

						path = Console.ReadLine();

						if (String.IsNullOrEmpty(path)) FileDelete(); else FileDelete(path);

						break;

					default:
						OPCION_EXISTE = false;
						break;
				}

				if (_opcion.KeyChar.ToString().Trim().ToUpper() == "X")
					break;

				if (OPCION_EXISTE)
				{
					Console.WriteLine("\n\nPress any key to continue...");
					Console.ReadKey();
				}
				else
					OPCION_EXISTE = true;
			}
		}
		
		static void FileDelete(string path = @"dir01/dir02/image.png")
		{
			try
			{
				var file = _share.GetRootDirectoryReference().GetFileReference(path);
				file.Delete();
				Console.WriteLine("\n\nFile [{0}] deleted...", path);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		static void FileDownload(string path = @"dir01/dir02/image.png")
		{
			try
			{
				var file = _share.GetRootDirectoryReference().GetFileReference(path);
				file.DownloadToFile(@"C:\Temp\AzureFiles\image.png", FileMode.Create);
				Console.WriteLine("\n\nFile [{0}] uploaded...", path);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		static void FileUpload(string path = @"C:\Users\japodaca\OneDrive\MCSD\Pictures\badge.png", string remotePath = @"/badge.png")
		{
			try
			{
				using (var fileStream = System.IO.File.OpenRead(path))
				{
					var file = _share.GetRootDirectoryReference().GetFileReference(remotePath);
					file.UploadFromStream(fileStream);
				}
				Console.WriteLine("\n\nFile [{0}] uploaded...", path);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		static void CreateShareFile()
		{
			try
			{
				#region Creating the Shared Files in Azure

				_storageAccount = CloudStorageAccount.Parse(
					CloudConfigurationManager.GetSetting("StorageConnection"));

				_share = _storageAccount.CreateCloudFileClient().GetShareReference("documentos");

				if (_share.Exists())
				{
					//Console.Clear();
					// Check current usage stats for the share.
					// Note that the ShareStats object is part of the protocol layer for the File service.
					ShareStats stats = _share.GetStats();
					//Console.WriteLine("Current share usage: {0} GB", stats.Usage.ToString());

					// Specify the maximum size of the share, in GB.
					// This line sets the quota to be 10 GB greater than the current usage of the share.
					_share.Properties.Quota = 10 + stats.Usage;
					_share.SetProperties();

					// Now check the quota for the share. Call FetchAttributes() to populate the share's properties.
					_share.FetchAttributes();
					//Console.WriteLine("Current share quota: {0} GB", _share.Properties.Quota);

					// Create a new shared access policy and define its constraints.
					SharedAccessFilePolicy sharedPolicy = new SharedAccessFilePolicy()
					{
						SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
						Permissions = SharedAccessFilePermissions.Read | SharedAccessFilePermissions.Write
					};

					// Get existing permissions for the share.
					FileSharePermissions permissions = _share.GetPermissions();

					if (!permissions.SharedAccessPolicies.ContainsKey("sampleSharePolicy"))
					{
						// Add the shared access policy to the share's policies. Note that each policy must have a unique name.
						permissions.SharedAccessPolicies.Add("sampleSharePolicy", sharedPolicy);
						_share.SetPermissions(permissions);
					}

					//Console.ReadKey();
				}
				else
					_share.CreateIfNotExists();

				#endregion
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.ReadKey();
			}
		}

		static void GetFilesAndDirectories(string path)
		{
			try
			{
				#region Listing files and directories at certain path

				Console.Clear();

				var directory = path.Trim() == "/" ? _share.GetRootDirectoryReference() : _share.GetRootDirectoryReference().GetDirectoryReference(path);

				if (directory.Exists())
				{
					Console.WriteLine(String.Format("{0, -60} {1, -10} {2, -30} {3, 20}", "Directory or File", "Type", "Content Type", "Size"));
					Console.WriteLine(String.Format("{0, -60} {1, -10} {2, -30} {3, -20}\n", (new String('=', 60)), (new String('=', 10)), (new String('=', 30)), (new String('=', 20))));
					foreach (var item in directory.ListFilesAndDirectories())
					{
						if (item.GetType() == typeof(CloudFile))
						{
							((CloudFile)item).FetchAttributes();
						}

						Console.WriteLine(String.Format("{0, -60} {1, -10} {2, -30} {3, 20}",
							item.GetType() == typeof(CloudFileDirectory) ? ((CloudFileDirectory)item).Name.Length > 60 ? ((CloudFileDirectory)item).Name.Substring(0, 30) : ((CloudFileDirectory)item).Name : ((CloudFile)item).Name.Length > 60 ? ((CloudFile)item).Name.Substring(0, 60) : ((CloudFile)item).Name,
							item.GetType() == typeof(CloudFileDirectory) ? "Directory" : "File",
							item.GetType() == typeof(CloudFile) ? ((CloudFile)item).Properties.ContentType.Length > 30 ? ((CloudFile)item).Properties.ContentType.Substring(0, 30) : ((CloudFile)item).Properties.ContentType : "",
							item.GetType() == typeof(CloudFile) ? String.Format("{0:N2} KB", (((CloudFile)item).Properties.Length / 1024)) : ""
							));
					}
				}
				else
				{
					Console.WriteLine("The directory [{0}], not exists...\n\n\n", path);
				}

				#endregion
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
