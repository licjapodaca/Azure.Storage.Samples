using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
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
			string path = "";

			while (_opcion.KeyChar.ToString().Trim().ToUpper() != "X")
			{
				Console.Clear();
				Console.WriteLine("(1) Listing files and directories from ROOT");
				Console.WriteLine("(2) Listing files and directories from a path");
				Console.WriteLine("(3) File upload");
				Console.WriteLine("(X) Exit\n\n");
				Console.Write("Opcion: ");
				_opcion = Console.ReadKey();

				switch (_opcion.KeyChar.ToString().Trim().ToUpper())
				{
					case "1":

						GetFilesAndDirectories("/");

						break;
					case "2":

						Console.Clear();
						Console.Write("\n\nType '/' if you want the root directory, otherwise type the name of the hierarchy directories...\n\nPath: ");

						path = Console.ReadLine();

						GetFilesAndDirectories(path);

						break;
					case "3":

						Console.Clear();
						Console.WriteLine("\n\nType the path name and file you want to upload...");
						Console.WriteLine("\nPath: ");

						path = Console.ReadLine();

						if (String.IsNullOrEmpty(path)) FileUpload(); else FileUpload(path);

						Console.WriteLine("\n\nFile [{0}] uploaded...", path);

						break;
				}

				if (_opcion.KeyChar.ToString().Trim().ToUpper() == "X")
					break;

				Console.WriteLine("\n\nPress any key to continue...");
				Console.ReadKey();
			}
		}

		static void FileUpload(string path = @"C:\Users\japodaca\OneDrive\MCSD\Pictures\badge.png")
		{
			try
			{
				var file = _share.GetRootDirectoryReference().GetDirectoryReference("dir01").GetFileReference("image.png");

				using (var fileStream = System.IO.File.OpenRead(path))
				{
					file.UploadFromStream(fileStream);
				}
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
				_share.CreateIfNotExists();

				#endregion
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
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
					Console.WriteLine(String.Format("{0, -60} {1, -10} {2, -30}", "Directory or File", "Type", "Content Type"));
					Console.WriteLine(String.Format("{0, -60} {1, -10} {2, -30}\n", (new String('=', 60)), (new String('=', 10)), (new String('=', 30))));
					foreach (var item in directory.ListFilesAndDirectories())
					{
						if(item.GetType() == typeof(CloudFile))
						{
							((CloudFile)item).FetchAttributes();
						}

						Console.WriteLine(String.Format("{0, -60} {1, -10} {2, -30}",
							item.GetType() == typeof(CloudFileDirectory) ? ((CloudFileDirectory)item).Name.Length > 60 ? ((CloudFileDirectory)item).Name.Substring(0, 30) : ((CloudFileDirectory)item).Name : ((CloudFile)item).Name.Length > 60 ? ((CloudFile)item).Name.Substring(0, 60) : ((CloudFile)item).Name,
							item.GetType() == typeof(CloudFileDirectory) ? "Directory" : "File",
							item.GetType() == typeof(CloudFile) ? ((CloudFile)item).Properties.ContentType.Length > 30 ? ((CloudFile)item).Properties.ContentType.Substring(0, 30) : ((CloudFile)item).Properties.ContentType : ""
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
