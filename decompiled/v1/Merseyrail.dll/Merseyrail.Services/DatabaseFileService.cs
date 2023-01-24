using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using Common.Services;
using Java.Util.Zip;
using SQLite;

namespace Merseyrail.Services;

public class DatabaseFileService
{
	private string tempFolder = Path.Combine(Path.GetTempPath(), "tempdb");

	private ISettingsService SettingService { get; set; }

	private IConnectivityService ConnectivityService { get; set; }

	public event EventHandler OnScheduleDbDownloadComplete;

	public event EventHandler OnStationDbDownloadComplete;

	public event EventHandler OnScheduleDbUpdateComplete;

	public event EventHandler OnStationDbUpdateComplete;

	public DatabaseFileService(ISettingsService settingService, IConnectivityService connectivityService)
	{
		SettingService = settingService;
		ConnectivityService = connectivityService;
	}

	public void UpdateScheduleDatabase()
	{
		string dbname = "schedule";
		string url = "https://merseyrail.app/data/schedule.zip";
		DownloadAndUnzipDb(dbname, url, "DB_LASTMODIFIED_FILE_SCHEDULE", "DB_FILE_SCHEDULE");
	}

	public void UpdateStationDatabase()
	{
		string dbname = "stations";
		string url = "https://merseyrail.app/data/stations.zip";
		DownloadAndUnzipDb(dbname, url, "DB_LASTMODIFIED_FILE_STATIONS", "DB_FILE_STATIONS");
	}

	private async void DownloadAndUnzipDb(string dbname, string url, string modifiedDateSetting, string currentDbSetting)
	{
		Console.WriteLine("DownloadAndUnzipDb: Line {0}", "44");
		if (ConnectivityService.IsConnected())
		{
			Console.WriteLine("DownloadAndUnzipDb: ConnectivityService.IsConnected(): Line {0}", "49");
			if (!Directory.Exists(tempFolder))
			{
				Console.WriteLine("DownloadAndUnzipDb: !Directory.Exists(tempFolder): Line {0}", "52");
				Directory.CreateDirectory(tempFolder);
			}
			else if (Directory.Exists(tempFolder))
			{
				Directory.Delete(tempFolder, recursive: true);
			}
			string setting = SettingService.GetSetting(modifiedDateSetting, string.Empty);
			Console.WriteLine("DownloadAndUnzipDb: string lastmodified = {1}: Line {0}", "57", setting);
			HttpWebRequest httpWebRequest = new HttpWebRequest(new Uri(url));
			if (!string.IsNullOrEmpty(setting))
			{
				httpWebRequest.IfModifiedSince = DateTime.Parse(setting);
			}
			await httpWebRequest.GetResponseAsync().ContinueWith(delegate(Task<WebResponse> r)
			{
				Console.WriteLine("DownloadAndUnzipDb: request.GetResponseAsync(): Line {0}", "66");
				if (r.IsCompleted)
				{
					Console.WriteLine("DownloadAndUnzipDb: r.IsCompleted: Line {0}", "68");
					try
					{
						string text = Path.Combine(tempFolder, dbname + ".zip");
						Console.WriteLine("DownloadAndUnzipDb: downloadZipPath={1}: Line {0}", "68", text);
						string fullName = new FileInfo(text).Directory.FullName;
						if (!Directory.Exists(fullName))
						{
							Directory.CreateDirectory(fullName);
						}
						using (FileStream destination = File.Create(text))
						{
							r.Result.GetResponseStream().CopyTo(destination);
							Console.WriteLine("DownloadAndUnzipDb: File.Create(downloadZipPath)={1}: Line {0}", "77", text);
						}
						string text2 = UnzipFiles(dbname, text);
						Console.WriteLine("DownloadAndUnzipDb: UnzipDatabase(dbname, downloadZipPath)={1}: Line {0}", "81", text2);
						if (text2 != null)
						{
							Console.WriteLine("DownloadAndUnzipDb: unzipped!=null: Line {0}", "83");
							ProcessNewDatabase(dbname, text2, modifiedDateSetting, currentDbSetting, ((HttpWebResponse)r.Result).LastModified);
						}
						else
						{
							Console.WriteLine("Unzip failed: {0}", dbname);
						}
					}
					catch (Exception ex)
					{
						if (ex.InnerException != null && ex.InnerException.Message.Contains("304"))
						{
							Console.WriteLine("No new " + dbname + " database available");
							if (this.OnScheduleDbUpdateComplete != null)
							{
								this.OnScheduleDbUpdateComplete(this, null);
							}
							if (this.OnStationDbUpdateComplete != null)
							{
								this.OnStationDbUpdateComplete(this, null);
							}
						}
						else
						{
							Console.WriteLine(ex.Message);
							if (this.OnScheduleDbUpdateComplete != null)
							{
								this.OnScheduleDbUpdateComplete(this, null);
							}
							if (this.OnStationDbUpdateComplete != null)
							{
								this.OnStationDbUpdateComplete(this, null);
							}
						}
					}
					finally
					{
						Console.WriteLine("DownloadAndUnzipDb: DeleteOldDatabases: Line {0}", "98");
						DeleteOldDatabases(dbname, SettingService.GetSetting(currentDbSetting, string.Empty));
						if (this.OnScheduleDbDownloadComplete != null)
						{
							this.OnScheduleDbDownloadComplete(this, null);
						}
						if (this.OnStationDbDownloadComplete != null)
						{
							this.OnStationDbDownloadComplete(this, null);
						}
					}
				}
			});
		}
		else
		{
			Console.WriteLine("Not connected");
		}
	}

	private void ProcessNewDatabase(string dbname, string unzippedFolder, string modifiedDateSetting, string currentDbSetting, DateTime lastModified)
	{
		Console.WriteLine("DownloadAndUnzipDb: ProcessNewDatabase: Line {0}", "127");
		DirectoryInfo directoryInfo = new DirectoryInfo(unzippedFolder);
		if ((from x in directoryInfo.GetFiles()
			where x.Name.Contains(".db")
			select x).Any())
		{
			Console.WriteLine("DownloadAndUnzipDb: zipdirinfo.GetFiles (): Line {0}", "131");
			_ = from x in directoryInfo.GetFiles()
				where x.Name.Contains(".db")
				select x;
			FileInfo fileInfo = (from x in directoryInfo.GetFiles()
				where x.Name.Contains(".db")
				orderby x.CreationTime descending
				select x).First();
			string path = string.Format("{0}_{1:yyyyMMddHHmmss}.db", fileInfo.Name.Replace(".db", ""), lastModified);
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path);
			Console.WriteLine("Source db exists {0}", File.Exists(fileInfo.FullName));
			File.Copy(fileInfo.FullName, text, overwrite: true);
			Console.WriteLine("New database copied from {0} to {1}", fileInfo.FullName, text);
			Console.WriteLine("Dest db exists {0}", File.Exists(text));
			using SQLiteConnection sQLiteConnection = new SQLiteConnection(text);
			if (sQLiteConnection.ExecuteScalar<string>("PRAGMA integrity_check;", Array.Empty<object>()) == "ok")
			{
				SettingService.SaveSetting(modifiedDateSetting, lastModified.ToString());
				Console.WriteLine("Saved modifiedDateSetting key:{0}, value{1}", modifiedDateSetting, lastModified.ToString());
				SettingService.SaveSetting(currentDbSetting, Utils.FullpathToAppFolderRelative(text));
				Console.WriteLine("Saved currentDbSetting key:{0}, value{1}", currentDbSetting, Utils.FullpathToAppFolderRelative(text));
				Console.WriteLine("new database saved setting {0}", Utils.FullpathToAppFolderRelative(text));
				if (this.OnScheduleDbUpdateComplete != null)
				{
					this.OnScheduleDbUpdateComplete(this, null);
				}
				if (this.OnStationDbUpdateComplete != null)
				{
					this.OnStationDbUpdateComplete(this, null);
				}
			}
			return;
		}
		Console.WriteLine("There was no database file in the downloaded zip archive.");
	}

	private static void DeleteOldDatabases(string dbname, string currentdbpath)
	{
		Console.WriteLine("DownloadAndUnzipDb: DeleteOldDatabases: Line {0}", "178");
		foreach (FileInfo item in from f in new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).GetFiles()
			where f.Name.StartsWith(dbname + "_") && f.Name != Path.GetFileName(currentdbpath)
			select f)
		{
			Console.WriteLine("Deleting unused db {0}", item.FullName);
			item.Delete();
		}
	}

	public void CleanTempFiles()
	{
		if (Directory.Exists(tempFolder))
		{
			Directory.Delete(tempFolder, recursive: true);
		}
		Directory.CreateDirectory(tempFolder);
	}

	private string UnzipFiles(string dbname, string zipfile)
	{
		string text = Path.Combine(tempFolder, dbname + "_temp");
		using ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(zipfile));
		ZipEntry nextEntry;
		while ((nextEntry = zipInputStream.NextEntry) != null)
		{
			string directoryName = Path.GetDirectoryName(nextEntry.Name);
			string fileName = Path.GetFileName(nextEntry.Name);
			directoryName = Path.Combine(text, directoryName);
			if (directoryName.Length > 0 && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			if (!(fileName != string.Empty))
			{
				continue;
			}
			using FileStream fileStream = File.Create(Path.Combine(text, nextEntry.Name));
			int num = 2048;
			byte[] array = new byte[num];
			while (true)
			{
				num = zipInputStream.Read(array, 0, array.Length);
				if (num > 0)
				{
					fileStream.Write(array, 0, num);
					continue;
				}
				break;
			}
		}
		return text;
	}

	public byte[] ReadFully(Stream input)
	{
		byte[] array = new byte[16384];
		using MemoryStream memoryStream = new MemoryStream();
		int count;
		while ((count = input.Read(array, 0, array.Length)) > 0)
		{
			memoryStream.Write(array, 0, count);
		}
		return memoryStream.ToArray();
	}

	private static Stream Decompress(byte[] input)
	{
		MemoryStream memoryStream = new MemoryStream();
		using (MemoryStream stream = new MemoryStream(input))
		{
			using DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
			deflateStream.CopyTo(memoryStream);
		}
		memoryStream.Position = 0L;
		return memoryStream;
	}
}
