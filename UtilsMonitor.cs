using System;
using System.IO;
using System.Text.RegularExpressions;

namespace HearthHelper
{
    internal class UtilsMonitor
    {
		public static string path1 = "";
		public static string path2 = "";
		public static string path3 = "";
		public static string path4 = "";
		public static string path5 = "";

		public static void CheckLogPath()
		{
			//日志路径
			path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
			if (!Directory.Exists(path1))
			{
				Directory.CreateDirectory(path1);
			}

			path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings/Default");
			if (!Directory.Exists(path2))
			{
				Directory.CreateDirectory(path2);
			}

			path3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
				"Routines/DefaultRoutine/Silverfish/UltimateLogs");
			if (!Directory.Exists(path3))
			{
				Directory.CreateDirectory(path3);
			}

			path4 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MercStraLogs");
			if (!Directory.Exists(path4))
			{
				Directory.CreateDirectory(path4);
			}
			
			path5 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Routines/BaconRoutine/Logs");
			if (!Directory.Exists(path5))
			{
				Directory.CreateDirectory(path5);
			}
		}

		//获取天梯文件最新写时间
		public static DateTime GetNormalFileLatestTime()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path3);
			DateTime dateTime = default(DateTime);
			try 
			{
				foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.txt"))
				{
					if (DateTime.Compare(fileInfo.LastWriteTime, dateTime) > 0)
					{
						dateTime = fileInfo.LastWriteTime;
					}
				}
			}
			catch { }
			return dateTime;
		}

		//获取佣兵文件最新写时间
		public static DateTime GetMercFileLatestTime()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path4);
			DateTime dateTime = default(DateTime);
			try 
			{
				foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.txt"))
				{
					if (DateTime.Compare(fileInfo.LastWriteTime, dateTime) > 0)
					{
						dateTime = fileInfo.LastWriteTime;
					}
				}
			}
			catch { }
			return dateTime;
		}
		
		//获取酒馆文件最新写时间
		public static DateTime GetBaconFileLatestTime()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path5);
			DateTime dateTime = default(DateTime);
			try 
			{
				foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.txt"))
				{
					if (DateTime.Compare(fileInfo.LastWriteTime, dateTime) > 0)
					{
						dateTime = fileInfo.LastWriteTime;
					}
				}
			}
			catch { }
			return dateTime;
		}
	}
}
