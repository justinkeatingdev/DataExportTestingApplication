using DataExport;
using Newtonsoft.Json;
using System.IO;

namespace DataExportTestingApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			var filePath = GetStaticFilePath();

			var aetnaData = DataExport.DataExportClass.RunData(filePath);

			if (aetnaData != null)
			{
				string jsonData = JsonConvert.SerializeObject(aetnaData);
				var nowTimeHour = DateTime.Now.Hour;
				var nowTimeminute = DateTime.Now.Minute;
				var nowTimeSeconds = DateTime.Now.Second;
				SaveJsonDataToAppData(jsonData, $"aetna_data-{nowTimeHour}-{nowTimeminute}-{nowTimeSeconds}.json");
			}
		}

		private static string GetStaticFilePath()
		{
			try
			{
				string appDataPath = GetAppDataFilePath();
				

				if (!string.IsNullOrEmpty(appDataPath))
				{
					string fileName = "test_eligibility_data.txt";
					string filePath = Path.Combine(appDataPath, fileName);
					if (File.Exists(filePath))
					{
						return filePath;
					}
					else
					{
						return null;
					}

				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		private static string GetAppDataFilePath()
		{
			try
			{
				string appDataPath = "";
				string currentPath = Directory.GetCurrentDirectory();

				int binIndex = currentPath.IndexOf("\\DataExportTestingApplication\\", StringComparison.OrdinalIgnoreCase);

				if (binIndex != -1)
				{
					appDataPath = currentPath.Substring(0, binIndex + "\\DataExportTestingApplication\\".Length) + "AppData";
				}

				appDataPath = appDataPath.Replace("DataExportTestingApplication\\AppData", "DataExportTestingApplication\\DataExportTestingApplication\\AppData");
				return appDataPath;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		private static void SaveJsonDataToAppData(string jsonData, string fileName)
		{
			try
			{
				string appDataPath = GetAppDataFilePath();

				if (!string.IsNullOrEmpty(appDataPath))
				{
					string filePath = Path.Combine(appDataPath, $"{fileName}");
					File.WriteAllText(filePath, jsonData);
				}
				else
				{
				}
			}
			catch (Exception ex)
			{
				Logger.LogInfo(String.Format($"JSON data FAILED \n"));
			}
		}
	}
}
