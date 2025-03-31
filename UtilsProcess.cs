using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Windows.Shapes;

namespace HearthHelper
{
    internal class UtilsProcess
    {
		//检测PID进程是否存在
		public static bool IsRunning(int pid)
		{
            Process[] processes = Process.GetProcessesByName("Hearthstone");
            if (processes != null && processes.Length > 0)
            {
                foreach (Process pro in processes)
                {
                    if (pid == pro.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
		}

		//停止战网和炉石
		public static void StopHearthstone(AccountItemWhole account,
			bool NeedMultStone, bool NeedCloseBattle, bool ForceCloseBattle)
		{
			UtilsCom.Log("准备停止战网、炉石、战网更新进程");
			try
			{
				//停止炉石
				UtilsCom.Log("停止炉石中...");
				Process[] processes = Process.GetProcessesByName("Hearthstone");
				if (processes != null && processes.Length > 0)
				{
					foreach (Process pro in processes)
					{
						if (NeedMultStone)
						{
							if (account.Running && account.StonePid == pro.Id)
							{
								pro.Kill();
								UtilsCom.Delay(1000);
								UtilsCom.Log($"检测到炉石(Pid={pro.Id})已停止");
								break;
							}
						}
						else
						{
							pro.Kill();
							UtilsCom.Delay(1000);
							UtilsCom.Log($"检测到炉石(Pid={pro.Id})已停止");
						}
					}
				}

				//停止战网
				if (NeedCloseBattle || ForceCloseBattle)
				{
					UtilsCom.Log("停止战网中...");
					processes = Process.GetProcessesByName("Battle.net");
					if (processes != null && processes.Length > 0)
					{
						foreach (Process pro in processes)
						{
							pro.Kill();
							UtilsCom.Delay(1000);
							UtilsCom.Log($"检测到战网(Pid={pro.Id})已停止");
						}
					}
				}

				//停止战网更新进程
				UtilsCom.Log("停止战网更新进程中...");
				processes = Process.GetProcessesByName("Agent");
				if (processes != null && processes.Length > 0)
				{
					foreach (Process pro in processes)
					{
						pro.Kill();
						UtilsCom.Delay(1000);
						UtilsCom.Log($"检测到战网更新进程(Pid={pro.Id})已停止");
					}
				}
			}
			catch { }
		}

		//启动战网
		[DllImport("kernel32.dll")]
		private static extern int WinExec(string exeName, int operType);
		public static void StartBattleNet(string BattleNetPath)
		{
			while (Process.GetProcessesByName("Battle.net").Length < 1)
			{
				WinExec(BattleNetPath, 2);
				UtilsCom.Delay(1000 * 5);
			}
		}

		//启动战网和炉石
		private static int startCnt=0;
		public static bool StartHearthstone(
			AccountItemWhole account,string HearthstonePath,
			ref bool IsRunning, bool NeedMultStone, bool NeedCloseBattle,
			bool ForceCloseBattle, bool needHang, string BattleNetPath, int BNHSInterval,
			int WindowWidth, int WindowHeigth,
            bool EnableHsMod, int HsModPort,
            bool EnableTimeGear, int NoFightTime,
            int PveFightTime, int PvpFightTime)
		{
			if (!IsRunning)
			{
				UtilsCom.Log($"用户主动停止运行，终止后续启动");
				return false;
			}

			try
			{
                var MyIni = new UtilsIniFile(System.IO.Path.Combine(HearthstonePath, "doorstop_config.ini"));
                string Arguments = "";
                string path = UtilsPath.GetBattleConfig();
                string json = File.ReadAllText(path);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
				string output;

                //HsMod插件开关
                if (EnableHsMod)
				{
                    MyIni.Write("enabled", "true", "UnityDoorstop");

                    //写入炉石启动命令行参数
                    if (account.currItem.Mode == (int)GameMode.ModeMerc)
                    {
                        Arguments += "--autostart";
                        if (needHang) Arguments += " --needHang";
                        if (account.currItem.MercConcede) Arguments += " --autoconcede";
                        if (account.currItem.MercCraft) Arguments += " --autocraft";
                        if (account.currItem.MercUpdate) Arguments += " --autoupdate";
                        if (account.StonePid != 0)
                        {
                            Arguments += " --pid:" + account.StonePid.ToString();
                        }
                        Arguments += " --rule:" + account.currItem.MercRule.ToString();
                        Arguments += " --behavior:" + account.currItem.MercBehavior.ToString();
                        if (!string.IsNullOrEmpty(account.currItem.MercTeam))
                        {
                            Arguments += " --team:" + account.currItem.MercTeam;
                        }
                        if (!string.IsNullOrEmpty(account.currItem.MercMap))
                        {
                            Arguments += " --map:" + account.currItem.MercMap;
                        }
                        Arguments += " --interval:" + account.currItem.MercInterval.ToString();
                        Arguments += " --path:" + AppDomain.CurrentDomain.BaseDirectory;
                        Arguments += " --hash:" + account.Email.GetHashCode().ToString();
                    }
                    if (EnableTimeGear)
                    {
                        Arguments += " --timeGear:" +
                            NoFightTime.ToString() + "," +
                            PveFightTime.ToString() + "," +
                            PvpFightTime.ToString();
                    }
                    Arguments += " --port:" + HsModPort.ToString();
                    Arguments += " --matchPath:" + System.IO.Path.Combine(
                        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MercInfo"),
                        "mercInfo" + account.Email.GetHashCode().ToString() + ".txt");
                    Arguments += " --width:" + WindowWidth.ToString();
                    Arguments += " --height:" + WindowHeigth.ToString();
                    Arguments += EnableHsMod ? " --afk:1" : " --afk:0";
                    jsonObj["Games"]["hs_beta"]["AdditionalLaunchArguments"] = Arguments;
                    output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    File.WriteAllText(path, output);
                }

				//启动炉石和启动战
				UtilsCom.Log("准备启动炉石");
				while (Process.GetProcessesByName("Battle.net").Length < 1 && IsRunning)
				{
					UtilsCom.Log($"未检测到战网，启动战网中...，5秒后再次检测");
					WinExec(BattleNetPath, 2);
					UtilsCom.Delay(1000 * 5);
				}
				if (!IsRunning)
				{
					UtilsCom.Log($"用户主动停止运行，终止后续启动");
					return false;
				}
				foreach (Process process in Process.GetProcessesByName("Battle.net"))
				{
					UtilsCom.Log($"已检测到战网(Pid={process.Id})运行中");
				}

				//启动炉石
				startCnt = 0;
				UtilsCom.Log($"{BNHSInterval}秒后启动炉石");
				UtilsCom.Delay(1000 * BNHSInterval);
				Process[] alreadyProcess = Process.GetProcessesByName("Hearthstone");
				while (Process.GetProcessesByName("Hearthstone").Length <=
					alreadyProcess.Length && IsRunning)
				{
					UtilsCom.Log($"未检测到炉石，启动炉石中...");
					if (NeedMultStone)
					{
						if (alreadyProcess.Length>0)
						{
                            UtilsCom.Log($"先停止战网更新进程...");
                            Process[] processes = Process.GetProcessesByName("Agent");
                            if (processes != null && processes.Length > 0)
                            {
                                foreach (Process pro in processes)
                                {
                                    pro.Kill();
                                    UtilsCom.Delay(1000);
                                    UtilsCom.Log($"检测到战网更新进程(Pid={pro.Id})已停止");
                                }
                            }
                            //再次检测战网更新进程
                            UtilsCom.Log($"再次检测战网更新进程...");
                            while (Process.GetProcessesByName("Agent").Length < 1 && IsRunning)
                            {
                                startCnt++;
                                UtilsCom.Log($"未检测到战网更新进程...，1秒后再次检测");
                                UtilsCom.Delay(1000);
                                if (startCnt > 200)
                                {
                                    UtilsCom.Log($"200秒内一直检测不到战网更新进程，停止后续启动");
                                    return false;
                                }
                            }
                            if (!IsRunning)
                            {
                                UtilsCom.Log($"用户主动停止运行，终止后续启动");
                                return false;
                            }
                            UtilsCom.Log($"检测到战网更新进程已运行，4秒后启动炉石...");
                            UtilsCom.Delay(4000);
                        }
						
					}
					Process[] array = Process.GetProcessesByName("Battle.net");
					int i = 0;
					if (i < array.Length)
					{
						Process.Start(array[i].MainModule.FileName, "--exec=\"launch WTCG\"");
					}
					UtilsCom.Delay(1000 * 5);
				}
				if (!IsRunning)
				{
					UtilsCom.Log($"用户主动停止运行，终止后续启动");
					return false;
				}
				foreach (Process allPro in Process.GetProcessesByName("Hearthstone"))
				{
					bool flag = true;
					foreach (Process alreadyPro in alreadyProcess)
					{
						if (allPro.Id == alreadyPro.Id) flag = false;
					}
					if (flag)
					{
						account.StonePid = allPro.Id;
						break;
					}
				}
				UtilsCom.Log($"已检测到炉石(Pid={account.StonePid})运行中");

				//停止战网进程
				if (NeedCloseBattle || ForceCloseBattle)
				{
					UtilsCom.Log("停止战网中...");
					while (Process.GetProcessesByName("Battle.net").Length != 0 && IsRunning)
					{
						foreach (Process process in Process.GetProcessesByName("Battle.net"))
						{
							try
							{
								process.Kill();
								UtilsCom.Delay(1000);
								UtilsCom.Log($"检测到战网(Pid={process.Id})已停止");
							}
							catch { }
						}
					}
					if (!IsRunning)
					{
						UtilsCom.Log($"用户主动停止运行，终止后续启动");
						return false;
					}
				}

				//停止战网更新进程
				UtilsCom.Log("停止战网更新进程中...");
				Process[] pros = Process.GetProcessesByName("Agent");
				if (pros != null && pros.Length > 0)
				{
					foreach (Process pro in pros)
					{
						pro.Kill();
						UtilsCom.Delay(1000);
						UtilsCom.Log($"检测到战网更新进程(Pid={pro.Id})已停止");
					}
				}

                //清空命令行参数，禁用插件启用
                if (EnableHsMod)
				{
                    json = File.ReadAllText(path);
                    jsonObj = JsonConvert.DeserializeObject(json);
                    jsonObj["Games"]["hs_beta"]["AdditionalLaunchArguments"] = "";
                    output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    File.WriteAllText(path, output);
                    MyIni.Write("enabled", "false", "UnityDoorstop");
                }
				return true;
			}
			catch (Exception ex)
			{
				UtilsCom.Log(ex.ToString());
				return false;
			}
		}

		//启动炉石兄弟
		public static void StartHearthbuddy(AccountItemWhole account,
			ref bool IsRunning, string HearthbuddyPath, int SystemVersion,
            int WindowWidth, int WindowHeigth)
		{
			if (!IsRunning)
			{
				UtilsCom.Log($"用户主动停止运行，终止后续启动");
				return;
			}

			UtilsCom.Log("准备启动炉石兄弟");
			try
			{
				//启动炉石兄弟
				UtilsCom.Log($"未检测到炉石兄弟，启动炉石兄弟中...");
				Process process = new Process();
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = HearthbuddyPath;
				process.StartInfo.Arguments = "--autostart --config:Default";
				
				if (account.currItem.Mode == (int)GameMode.ModeBattle)
				{
					process.StartInfo.Arguments += " --bot:酒馆战棋";
					process.StartInfo.Arguments += " --routine:酒馆策略";
				}
				else
				{
					process.StartInfo.Arguments += " --bot:天梯脚本";
					process.StartInfo.Arguments += " --routine:天梯策略";
				}

				if (account.StonePid != 0)
				{
					process.StartInfo.Arguments += " --pid:" + account.StonePid.ToString();
				}
				if (!string.IsNullOrEmpty(account.currItem.NormalDeck))
				{
					process.StartInfo.Arguments += " --deck:" + account.currItem.NormalDeck;
				}
				process.StartInfo.Arguments += " --behavior:" + account.currItem.NormalBehavior.ToString();
				process.StartInfo.Arguments += " --rule:" + account.currItem.NormalRule.ToString();
                process.StartInfo.Arguments += " --width:" + WindowWidth.ToString();
                process.StartInfo.Arguments += " --height:" + WindowHeigth.ToString();
                process.StartInfo.Arguments += " --os:" + (SystemVersion == 0 ? "10" : "7");
				process.Start();
				UtilsCom.Log("已经启动炉石兄弟，是否成功，听天由命~");
			}
			catch (Exception ex)
			{
				UtilsCom.Log(ex.ToString());
			}
		}
	}
}
