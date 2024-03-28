using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DataExport;

namespace DataTester
{
	[TestClass]
	public class DataTest1
	{
		private const string FilePath = "D:\\CODING\\The Helper Bees\\justin_keating__thb\\TheHelperBeesConsoleApp\\AppData\\test_eligibility_data.txt";
		private DetailData DetailData result = AetnaEligibilityFileIngestV2.RunAetnaFileIngest(FilePath);

		[TestMethod]
		public void TestRunAetnaFileIngest()
		{
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void TestCheckDetailRecordsCount()
		{
			var expectedDetailRecordCount = result.Trailer.RecordCount;

			var detailRecordCount = result.DetailsRecords.Count;

			Assert.AreEqual(expectedDetailRecordCount, detailRecordCount);
		}

		[TestMethod]
		public void TestAetnaFileIngestObject()
		{
			const int expectedAetnaObjectCount = 3;

			int resultObjectCount = 0;
			if (result.Header != null)
			{
				resultObjectCount++;
			}
			if (result.DetailsRecords != null && result.DetailsRecords.Count > 0)
			{
				resultObjectCount++;
			}
			if (result.Trailer != null)
			{
				resultObjectCount++;
			}

			Assert.AreEqual(expectedAetnaObjectCount, resultObjectCount);
		}

		[TestMethod]
		public void TestFileNotEmpty()
		{
			if (File.Exists(FilePath))
			{
				string fileContent = File.ReadAllText(FilePath);

				Assert.IsFalse(string.IsNullOrEmpty(fileContent), "File is empty");
			}
			else
			{
				Assert.Fail("File does not exist");
			}
		}
	}
}
