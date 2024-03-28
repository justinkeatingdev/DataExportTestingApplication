using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using log4net;
using log4net.Repository.Hierarchy;
using System.Runtime.CompilerServices;


namespace DataExport
{
	public class Logger
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

		public static void LogInfo(string message)
		{
			log.Info(message);
			Console.WriteLine(message);
		}

		public static void LogWarning(string message)
		{
			log.Warn(message);
			Console.WriteLine(message);
		}

		public static void LogError(string message)
		{
			log.Error(message);
			Console.WriteLine(message);
		}

		public static void LogFatal(string message)
		{
			log.Fatal(message);
			Console.WriteLine(message);
		}
	}
}
