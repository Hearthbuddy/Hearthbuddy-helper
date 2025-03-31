using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace HearthHelper
{
    internal class UtilsPath
    {
		//从注册表中寻找安装路径
		public static string FindInstallPathFromRegistry(string uninstallKeyName)
		{
			try
			{
				RegistryKey key = Registry.LocalMachine.OpenSubKey(
					$@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{uninstallKeyName}");
				if (key == null) return null;
				object installLocation = key.GetValue("InstallLocation");
				key.Close();
				if (installLocation != null && !string.IsNullOrEmpty(installLocation.ToString()))
				{
					return installLocation.ToString();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			return null;
		}

		//选择文件，返回文件完整路径
		public static string SelectExeFile(string filter)
		{
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
			{
				Filter = filter,
				DereferenceLinks = false
			};
			if (openFileDialog.ShowDialog() == true)
			{
				return openFileDialog.FileName;
			}
			return string.Empty;
		}

        //选文件夹，返回完整路径
        public static string SelectBrowser()
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                return folderBrowserDialog.SelectedPath;
            }
            return string.Empty;
        }

		//自动获取战网路径
		public static string AutoGetBattleNetPath()
		{
			string BattleNetPath = FindInstallPathFromRegistry("Battle.net");
			if (!string.IsNullOrEmpty(BattleNetPath) && Directory.Exists(BattleNetPath)
				&& File.Exists(Path.Combine(BattleNetPath, "Battle.net Launcher.exe")))
			{
				string path = Path.Combine(BattleNetPath, "Battle.net Launcher.exe");
				UtilsCom.Log("自动获取战网路径成功：");
				UtilsCom.Log(path);
				return path;
			}
			UtilsCom.Log("自动获取战网路径失败，请手动选择");
			return string.Empty;
		}

		//自动获取炉石路径
		public static string AutoGetHearthstonePath()
		{
			string HearthstonePath = FindInstallPathFromRegistry("Hearthstone");
			if (!string.IsNullOrEmpty(HearthstonePath) && Directory.Exists(HearthstonePath)
				&& File.Exists(Path.Combine(HearthstonePath, "Hearthstone.exe")))
			{
				string path = HearthstonePath;
				UtilsCom.Log("自动获取炉石路径成功：");
				UtilsCom.Log(path);
				return path;
			}
			UtilsCom.Log("自动获取炉石路径失败，请手动选择");
			return string.Empty;
		}

		//自动获取兄弟路径
		public static string AutoGetHearthBuddyPath()
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			if (!string.IsNullOrEmpty(currentDirectory) && Directory.Exists(currentDirectory)
				&& File.Exists(Path.Combine(currentDirectory, "Hearthbuddy.exe")))
			{
				string path = Path.Combine(currentDirectory, "Hearthbuddy.exe");
				UtilsCom.Log("自动获取兄弟路径成功：");
				UtilsCom.Log(path);
				return path;
			}
			UtilsCom.Log("自动获取兄弟路径失败，请检测本程序是否在兄弟目录下");
			return string.Empty;
		}

		//获取战网配置文件
		public static string GetBattleConfig()
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath)
				&& Directory.Exists(Path.Combine(folderPath, "Battle.net"))
				&& File.Exists(Path.Combine(folderPath, "Battle.net", "Battle.net.config")))
			{
				return Path.Combine(folderPath, "Battle.net", "Battle.net.config");
			}
			return string.Empty;
		}
	}
}

