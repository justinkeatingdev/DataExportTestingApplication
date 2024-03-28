using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace DataExport
{
	public class DataExportClass
	{
		public static DetailData RunData(string filePath)
		{
			DetailData aetnaData = new DetailData();

			var stream = new StreamReader(filePath);
			int rowsInFile = TotalLines(filePath);
			var firstLine = stream.ReadLine();
			//var secondLine = firstLine.Contains("HDR") ? firstLine : stream.ReadLine();
			string lastLine = "";

			List<string> linesTemp = new List<string>();
			string currentLine;
			while (!stream.EndOfStream)
			{
				currentLine = stream.ReadLine();
				if (currentLine != firstLine)
				{
					if (!stream.EndOfStream)
					{
						linesTemp.Add(currentLine);
					}
					else
					{
						lastLine = currentLine;
					}
				}
			}

			try
			{
				var headerData = CreateHeaderRecord(firstLine);
				var trailerData = CreateTrailerRecord(lastLine);
				var detailRecords = GetAllDetailRecords(linesTemp);


				if (headerData != null && detailRecords != null && trailerData != null)
				{
					Logger.LogInfo(String.Format($"**Started AetnaData Creation**"));
					Logger.LogInfo(String.Format($"**Started AetnaData Creation**"));

					aetnaData = new DetailData
					{
						Header = headerData,
						DetailsRecords = detailRecords,
						Trailer = trailerData
					};

					if (aetnaData != null)
					{
						Logger.LogInfo(String.Format($"Aetna Data Creation was Successful"));
					}
					else
					{
						Logger.LogInfo(String.Format($"Aetna Data Creation FAILED"));
					}

					Logger.LogInfo(String.Format($"**Ended Aetna Data Creation** \n"));
				}
				else
				{
					Logger.LogInfo(String.Format($"FAILED Aetna Data Creation"));
				}
			}
			catch (Exception ex)
			{
				Logger.LogInfo(String.Format($"Failure to Create Aetna Object - {ex.Message}"));
			}

			stream.Dispose();

			return aetnaData;
		}

		static int TotalLines(string filePath)
		{
			using (StreamReader r = new StreamReader(filePath))
			{
				int i = 0;
				while (r.ReadLine() != null) { i++; }
				return i;
			}
		}

		static HeaderRecord CreateHeaderRecord(string firstLine)
		{
			if (!string.IsNullOrEmpty(firstLine))
			{
				try
				{
					Logger.LogInfo(String.Format($"**Started Header Record Creation**"));

					string[] parts = firstLine.Split(',');

					HeaderRecord headerRecord = new HeaderRecord
					{
						RecordType = ParseString(parts[0], nameof(HeaderRecord.RecordType)),
						SenderName = ParseString(parts[1], nameof(HeaderRecord.SenderName)),
						ContactInfo = ParseString(parts[2], nameof(HeaderRecord.ContactInfo)),
						CreateDate = ParseDateTime(parts[3], nameof(HeaderRecord.CreateDate)),
						CreateTime = ParseTime(parts[4], nameof(HeaderRecord.CreateTime))
					};

					Logger.LogInfo(String.Format($"**Ended Header Record Creation** \n"));

					return headerRecord;
				}
				catch (Exception ex)
				{
					Logger.LogInfo(String.Format($"Error Message for HeaderRecord Creation - {ex.Message}"));
				}
			}
			else
			{
				Logger.LogInfo(String.Format($"Firstline was empty"));
			}

			return null;
		}

		static TrailerRecord CreateTrailerRecord(string lastLine)
		{
			if (!string.IsNullOrEmpty(lastLine))
			{
				try
				{

					Logger.LogInfo(String.Format($"**Started Trailer Record Creation**"));

					string[] parts = lastLine.Split(',');

					TrailerRecord trailerRecord = null;

					if (parts != null && parts.Count() > 0)
					{
						if (parts.Length > 2)
						{
							trailerRecord = new TrailerRecord
							{
								RecordType = ParseString(parts[0], nameof(TrailerRecord.RecordType)),
								FillerOne = ParseString(parts[1], nameof(TrailerRecord.FillerOne)),
								RecordCount = ParseLong(parts[2], nameof(TrailerRecord.RecordCount))
							};
						}
						else
						{
							trailerRecord = new TrailerRecord
							{
								RecordType = ParseString(parts[0], nameof(TrailerRecord.RecordType)),
								RecordCount = ParseLong(parts[1], nameof(TrailerRecord.RecordCount))
							};
						}
					}

					Logger.LogInfo(String.Format($"**Ended Trailer Record Creation** \n"));

					return trailerRecord;
				}
				catch (Exception ex)
				{
					Logger.LogInfo(String.Format($"Error Message for TrailerRecord Creation - {ex.Message}"));
				}
			}
			else
			{
				Logger.LogInfo(String.Format($"Lastline was empty"));
			}

			return null;
		}

		static DetailRecord CreateDetailRecord(string detailLine, int lineNumber)
		{
			try
			{
				var values = detailLine.Split('~');

				DetailRecord detailRecord = null;

				if (values.Length < 32)
				{
					Logger.LogInfo("Not enough data from line to create DetailRecord");
					return null;
				}

				if (values != null && values.Count() > 0)
				{
					detailRecord = new DetailRecord
					{
						MemberID = ParseString(values[0], nameof(DetailRecord.MemberID)),
						Birthdate = ParseDateTime(values[1], nameof(DetailRecord.Birthdate)),
						Gender = ParseString(values[2], nameof(DetailRecord.Gender)),
						EffectiveDate = ParseDateTime(values[3], nameof(DetailRecord.EffectiveDate)),
						TerminationDate = ParseDateTime(values[4], nameof(DetailRecord.TerminationDate)),
						FirstName = ParseString(values[5], nameof(DetailRecord.FirstName)),
						MI = ParseString(values[6], nameof(DetailRecord.MI)),
						LastName = ParseString(values[7], nameof(DetailRecord.LastName)),
						Mailing_Address1 = ParseString(values[8], nameof(DetailRecord.Mailing_Address1)),
						Mailing_Address2 = ParseString(values[9], nameof(DetailRecord.Mailing_Address2)),
						Mailing_City = ParseString(values[10], nameof(DetailRecord.Mailing_City)),
						Mailing_State = ParseString(values[11], nameof(DetailRecord.Mailing_State)),
						Mailing_Zipcode = ParseLong(values[12], nameof(DetailRecord.Mailing_Zipcode)),
						Residential_Address1 = ParseString(values[13], nameof(DetailRecord.Residential_Address1)),
						Residential_Address2 = ParseString(values[14], nameof(DetailRecord.Residential_Address2)),
						Residential_City = ParseString(values[15], nameof(DetailRecord.Residential_City)),
						Residential_State = ParseString(values[16], nameof(DetailRecord.Residential_State)),
						Residential_Zipcode = ParseLong(values[17], nameof(DetailRecord.Residential_Zipcode)),
						PhoneNumber = ParseLong(values[18], nameof(DetailRecord.PhoneNumber)),
						AlternatePhoneNbr = ParseLong(values[19], nameof(DetailRecord.AlternatePhoneNbr)),
						Member_Language = ParseString(values[20], nameof(DetailRecord.Member_Language)),
						Contract = ParseString(values[21], nameof(DetailRecord.Contract)),
						PBP = ParseLong(values[22], nameof(DetailRecord.PBP)),
						Plan_Eff_Date = ParseDateTime(values[23], nameof(DetailRecord.Plan_Eff_Date)),
						Plan_name = ParseString(values[24], nameof(DetailRecord.Plan_name)),
						Company_Code_Group_Number = ParseString(values[25], nameof(DetailRecord.Company_Code_Group_Number)),
						Group_Name = ParseString(values[26], nameof(DetailRecord.Group_Name)),
						FillerOneEmployeeID = ParseString(values[27], nameof(DetailRecord.FillerOneEmployeeID)),
						FillerTwo = ParseString(values[28], nameof(DetailRecord.FillerTwo)),
						FillerThree = ParseString(values[29], nameof(DetailRecord.FillerThree)),
						FillerFour = ParseString(values[30], nameof(DetailRecord.FillerFour)),
						FillerFive = ParseString(values[31], nameof(DetailRecord.FillerFive))
					};
				}

				Logger.LogInfo(String.Format($"Created DetailRecord data finished for line : {lineNumber}"));
				return detailRecord;
			}
			catch (Exception ex)
			{
				Logger.LogInfo(String.Format($"Error Message for DetailRecord Creation - {ex.Message}"));
				return null;
			}
		}

		static List<DetailRecord> GetAllDetailRecords(List<string> lines)
		{
			List<DetailRecord> detailRecords = new List<DetailRecord>();

			if (lines != null && lines.Any())
			{
				int lineCounter = 2;
				foreach (var line in lines)
				{
					Logger.LogInfo(String.Format($"**Started Line Detail Record - {lineCounter}**"));

					var lineDetailRecord = CreateDetailRecord(line, lineCounter);
					if (lineDetailRecord != null)
					{
						detailRecords.Add(lineDetailRecord);

						Logger.LogInfo(String.Format($"**Ended Line Detail Record - {lineCounter}** \n"));
					}

					lineCounter++;
				}
			}

			return detailRecords;
		}

		static string ParseString(string value, string propertyName = "")
		{

			Logger.LogInfo(String.Format($"Successfully Parsing String : '{value}' for property -> {propertyName}"));

			return value ?? string.Empty;
		}

		static long? ParseLong(string value, string propertyName = "")
		{
			if (string.IsNullOrEmpty(value))
			{
				Logger.LogInfo(String.Format($"Successfully Parsing long : '{value}' for property -> {propertyName}"));
				return null;
			}

			long result;
			if (!long.TryParse(value, out result))
			{
				Logger.LogInfo($"FAILED to parse long value: '{value}' for property -> {propertyName}");
				return null;
			}
			else
			{
				Logger.LogInfo(String.Format($"Successfully Parsing long : '{value}' for property -> {propertyName}"));
			}

			return result;
		}

		static DateTime? ParseDateTime(string value, string propertyName = "")
		{
			if (string.IsNullOrEmpty(value))
			{
				Logger.LogInfo(String.Format($"Successfully Parsing DateTime : '{value}' for property -> {propertyName}"));
				return null;
			}

			DateTime result;

			if (!DateTime.TryParseExact(value, "MMddyyyy", null, DateTimeStyles.None, out result))
			{
				Logger.LogInfo($"FAILED to parse DateTime value: '{value}' for property -> {propertyName}");
				return null;
			}
			else
			{
				Logger.LogInfo(String.Format($"Successfully Parsing DateTime : '{value}' for property -> {propertyName}"));
			}

			return result;
		}

		static TimeSpan? ParseTime(string value, string propertyName = "")
		{
			if (string.IsNullOrEmpty(value))
			{
				Logger.LogInfo(String.Format($"Successfully Parsing Time : '{value}' for property -> {propertyName}"));
				return null;
			}

			TimeSpan result;

			if (!TimeSpan.TryParseExact(value, "hhmmss", null, out result))
			{
				Logger.LogInfo($"FAILED to parse Time value: '{value}' for property -> {propertyName}");
				return null;
			}
			else
			{
				Logger.LogInfo(String.Format($"Successfully Parsing Time : '{value}' for property -> {propertyName}"));
			}

			return result;
		}
	}



	public class DetailData
	{
		public HeaderRecord Header { get; set; }
		public List<DetailRecord> DetailsRecords { get; set; }
		public TrailerRecord Trailer { get; set; }
	}

	public class DetailRecord
	{
		public string MemberID { get; set; }
		public DateTime? Birthdate { get; set; }
		public string Gender { get; set; }
		public DateTime? EffectiveDate { get; set; }
		public DateTime? TerminationDate { get; set; }
		public string FirstName { get; set; }
		public string MI { get; set; }
		public string LastName { get; set; }
		public string Mailing_Address1 { get; set; }
		public string Mailing_Address2 { get; set; }
		public string Mailing_City { get; set; }
		public string Mailing_State { get; set; }
		public long? Mailing_Zipcode { get; set; }
		public string Residential_Address1 { get; set; }
		public string Residential_Address2 { get; set; }
		public string Residential_City { get; set; }
		public string Residential_State { get; set; }
		public long? Residential_Zipcode { get; set; }
		public long? PhoneNumber { get; set; }
		public long? AlternatePhoneNbr { get; set; }
		public string Member_Language { get; set; }
		public string Contract { get; set; }
		public long? PBP { get; set; }
		public DateTime? Plan_Eff_Date { get; set; }
		public string Plan_name { get; set; }
		public string Company_Code_Group_Number { get; set; }
		public string Group_Name { get; set; }
		public string FillerOneEmployeeID { get; set; }
		public string FillerTwo { get; set; }
		public string FillerThree { get; set; }
		public string FillerFour { get; set; }
		public string FillerFive { get; set; }
	}

	public class HeaderRecord
	{
		public string RecordType { get; set; }
		public string SenderName { get; set; }
		public string ContactInfo { get; set; }
		public DateTime? CreateDate { get; set; }
		public TimeSpan? CreateTime { get; set; }
	}

	public class TrailerRecord
	{
		public string RecordType { get; set; }
		public string FillerOne { get; set; }
		public long? RecordCount { get; set; }
	}
}
