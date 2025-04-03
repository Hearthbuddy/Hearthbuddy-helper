using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Xml;
using System.Windows.Shapes;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Threading;
using System.Security.Cryptography;

namespace HearthHelper
{
	public partial class MainWindow : Window
	{
		public ObservableCollection<AccountItemWhole> AccountList = 
			new ObservableCollection<AccountItemWhole>();
		private readonly System.Windows.Forms.Timer TimerLog =
			new System.Windows.Forms.Timer();
		private readonly System.Windows.Forms.Timer TimerTick = 
			new System.Windows.Forms.Timer();
		private readonly System.Windows.Forms.Timer TimerMonitor =
			new System.Windows.Forms.Timer();
		private readonly System.Windows.Forms.Timer TimerInterval =
			new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer TimerInit =
            new System.Windows.Forms.Timer();
        private DispatcherTimer timerRun1;
        private DispatcherTimer timerRun2;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private AccountItemWhole CurrRunningAccount =
			new AccountItemWhole(false, "");
		private bool ForceCloseBattle = false;
		private bool IsRunning = false;
		private int TodayRebootCnt = 0;
		private DateTime RunDateTime = default(DateTime);
		private long LastPushNormalDateTime = 
			DateTime.Now.ToBinary()/1000/10000-24*3600;
		private int checkUpdateInterval = 1000*3600;
        private int killInterval = 60 * 1000;
        private const string MyVersion = "1.0.3";
		private const string MyTitlt = "中控"+"--[2025-02-22]--";
        public MainWindow()
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen;// 窗体居中
			InitializeComponent();
		}
		
		private string BattleNetPath
		{
			get{return BattleNetPathTextBox.Text;}
			set{BattleNetPathTextBox.Text = value;}
		}
		private string HearthstonePath
		{
			get{return HearthstonePathTextBox.Text;}
			set{HearthstonePathTextBox.Text = value;}
		}
		private string HearthbuddyPath
		{
			get{return HearthbuddyPathTextBox.Text;}
			set{HearthbuddyPathTextBox.Text = value;}
		}
		private int BNHSInterval
		{
			get{int x; return int.TryParse(BNHSIntervalTextBox.Text,out x)?
					x: UtilsConfig.DefaultBNHSInterval; }
			set{BNHSIntervalTextBox.Text = value.ToString(); }
		}
		private int HSHBInterval
		{
			get{int x;return int.TryParse(HSHBIntervalTextBox.Text,out x)?
					x : UtilsConfig.DefaultHSHBInterval;}
			set{HSHBIntervalTextBox.Text = value.ToString(); }
		}
		private int CheckInterval
		{
			get{int x; return int.TryParse(CheckIntervalTextBox.Text,out x)?
					x : UtilsConfig.DefaultCheckInterval;}
			set{CheckIntervalTextBox.Text = value.ToString(); }
		}
		private bool NeedCloseBattle
		{
			get{return (bool)NeedCloseBattleCheckBox.IsChecked; }
			set{NeedCloseBattleCheckBox.IsChecked = value; }
		}
		private bool NeedMultStone
		{
			get{return (bool)NeedMultStoneCheckBox.IsChecked; }
			set{NeedMultStoneCheckBox.IsChecked = value; }
		}
		private string TodayRebootCntStr
		{
			set{TodayRebootCntLabel.Content = value; }
		}
		private int RebootCntMax
		{
			get{int x; return int.TryParse(RebootMaxCntTextBox.Text,out x)?
					x : UtilsConfig.DefaultCheckInterval; }
			set{RebootMaxCntTextBox.Text = value.ToString(); }
		}
		private int SystemVersion
		{
			get{return SystemVersionComboBox.SelectedIndex; }
			set{SystemVersionComboBox.SelectedIndex = value; }
		}
		private int WindowWidth
		{
			get
			{
				int w; return int.TryParse(WindowWidthTextBox.Text, out w) ?
					w : UtilsConfig.DefaultWindowWidth;
			}
			set { WindowWidthTextBox.Text = value.ToString(); }
		}
		private int WindowHeight
		{
			get
			{
				int h; return int.TryParse(WindowHeightTextBox.Text, out h) ?
					h : UtilsConfig.DefaultWindowHeight;
			}
			set { WindowHeightTextBox.Text = value.ToString(); }
		}
		private string PushPlusToken
		{
			get{return PushPlusTokenTextBox.Text; }
			set{PushPlusTokenTextBox.Text = value; }
		}
		private bool NeedPushMessage
		{
			get{return (bool)NeedPushMessageCheckBox.IsChecked; }
			set{NeedPushMessageCheckBox.IsChecked = value; }
		}
		private bool NeedPushNormal
		{
			get{return (bool)NeedPushNormalCheckBox.IsChecked; }
			set{NeedPushNormalCheckBox.IsChecked = value; }
		}
		private int PushNormalInterval
		{
			get{int x; return int.TryParse(PushNormalIntervalTextBox.Text,out x)?
				 x : UtilsConfig.DefaultPushNormalInterval; }
			set{PushNormalIntervalTextBox.Text = value.ToString(); }
		}
        public bool EnableHsMod
        {
            get { return (bool)EnableHsModCheckBox.IsChecked; }
            set { EnableHsModCheckBox.IsChecked = value; }
        }
        public int HsModPort
        {
            get
            {
                int x; return int.TryParse(HsModPortTextBox.Text, out x) ?
                    x : UtilsConfig.DefaultHsModPort;
            }
            set { HsModPortTextBox.Text = value.ToString(); }
        }
        public bool EnableGMMessageShow
        {
            get { return (bool)EnableGMMessageShowCheckBox.IsChecked; }
            set { EnableGMMessageShowCheckBox.IsChecked = value; }
        }

        public bool EnableEnemyEmote
        {
	        get { return (bool)EnableEnemyEmoteCheckBox.IsChecked; }
	        set { EnableEnemyEmoteCheckBox.IsChecked = value; }
        }
        
        public bool EnableQuickMode
        {
	        get { return (bool)EnableQuickModeCheckBox.IsChecked; }
	        set { EnableQuickModeCheckBox.IsChecked = value; }
        }
        
        public bool EnableRankInGameShow
        {
	        get { return (bool)EnableRankInGameShowCheckBox.IsChecked; }
	        set { EnableRankInGameShowCheckBox.IsChecked = value; }
        }
        
        public bool EnableCardState
        {
	        get { return (bool)EnableCardStateCheckBox.IsChecked; }
	        set { EnableCardStateCheckBox.IsChecked = value; }
        }
        
        //显示日志
        private static int logCnt = 0;
		private void ShowLog(object sender, EventArgs e)
		{
			if (UtilsCom.logQueue.Count > 0)
			{
				string log = (string)UtilsCom.logQueue.Dequeue();
				if (logCnt++ > 500)
				{
					logCnt = 0;
					LogTextBox.Clear();
				}
				LogTextBox.AppendText(DateTime.Now.ToLongTimeString() + "：" +
					(NeedMultStone ? "[多]" : "[单]") + log + "\n");
                if (logCnt > 60) LogTextBox.ScrollToEnd();
			}
		}

		private void IntervalCheck()
		{
			try
			{
				string newVersion="";
                string date = "";
                string downloadurl = "";
				string downloadname = "";
            	string describe = "";
				string must = "";
                string auth = "";
                string enable = "";
				string reason = "";
				int result=SoftUpdate.bNeedUpdate(
					MyVersion,ref newVersion,ref date,
					ref downloadurl,ref downloadname,ref describe,
					ref must, ref auth, ref enable,ref reason);
				if(-1==result) 
				{
					UtilsCom.Log(describe);
					UtilsCom.Log($"{killInterval / 1000}秒后本程序将关闭！");
					UtilsCom.Delay(killInterval);
                    WindowExit();
                }
				if(enable!="1")
				{
					UtilsCom.Log(reason);
					UtilsCom.Log($"{killInterval / 1000}秒后本程序将关闭！");
					UtilsCom.Delay(killInterval);
                    WindowExit();
                }
				if (0 == result)
				{
					if (System.Windows.MessageBox.Show(
						$"有新版本：V{newVersion}\n更新日期：{date}\n\n更新内容如下：\n{describe}\n\n是否立即更新？", "更新",
						MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						//启动下载线程
						new Thread(new ParameterizedThreadStart(
							SoftUpdate.DownloadFile)).Start(downloadurl + "@" + downloadname);

						//检查是否文件下载完毕
						while (true)
						{
							if (SoftUpdate.bDownloadFileOk())
							{
								//关闭炉石兄弟等
								UtilsProcess.StopHearthstone(CurrRunningAccount,
									NeedMultStone, NeedCloseBattle, ForceCloseBattle);

								//启动升级程序
								Process process = new Process();
								process.StartInfo.FileName = downloadname;
								process.Start();

                                //本程序退出
                                WindowExit();
                            }
							else UtilsCom.Delay(1000);
						}
					}
					else
					{
						if (must == "1")
						{
							UtilsCom.Log($"新版本V{newVersion}必须更新，否则无法运行！");
							UtilsCom.Log($"{killInterval / 1000}秒后本程序将关闭！");
							UtilsCom.Delay(killInterval);
                            WindowExit();
                        }
					}
				}
				else
				{
                    UtilsCom.Log("当前版本已是最新，无需更新！");
					if (auth=="1")
					{
                        if (0 != Security.bAuthOK())
                        {
                            //Process.Start("https://gitee.com/UniverseString/Hearthstone-myAccount");
                            UtilsCom.Log($"{killInterval / 1000}秒后本程序将关闭！");
                            UtilsCom.Delay(killInterval);
                            WindowExit();
                        }
                    }
					else UtilsCom.Log("当前用户认证成功，正常使用！");
                }
			}
			catch (Exception)
			{
				UtilsCom.Log("启动错误，请检查网络是否正常！");
				UtilsCom.Log($"{killInterval/1000}秒后本程序将关闭！");
				UtilsCom.Delay(killInterval);
                WindowExit();
            }
		}

		//程序升级
		private void MyUpdate(object sender, EventArgs e)
		{
            TimerInterval.Stop();
            IntervalCheck();
            TimerInterval.Interval = checkUpdateInterval;
            TimerInterval.Start();
		}

		//监控程序
		private void MyMonitor(object sender, EventArgs e)
		{
			//判断时间，重置TodayRebootCnt
			TimeSpan nowDt = DateTime.Now.TimeOfDay;
			TimeSpan workStart = DateTime.Parse("00:00:00").TimeOfDay;
			TimeSpan workStop = DateTime.Parse("00:00:05").TimeOfDay;
			if (nowDt > workStart && nowDt < workStop)
			{
				UtilsCom.Log($"当前时间0点0分，重启次数重置为0");
				TodayRebootCnt = -1;
				TimerMonitor.Stop();
				UtilsCom.Delay(1000*6);
				TimerMonitor.Interval = 1000;
				TimerMonitor.Start();
			}
			if (TodayRebootCnt==-1)
			{
				TodayRebootCnt = 0;
				TodayRebootCntStr = TodayRebootCnt.ToString() + "/";
				if (!IsRunning)
				{
					if (NeedPushMessage)
					{
						string result;
						UtilsPush.PostMessage(PushPlusToken,"",
							CurrRunningAccount.Email, "", TodayRebootCnt, RebootCntMax,
							UtilsPush.MSG_TYPE.MSG_START, out result);
						UtilsCom.Log(result);
					}
					UtilsCom.Log($"一日之计在于晨，兄弟中控自动开始运行");
					StartRun();
				}
			}
			if (IsRunning) LogTextBox.ScrollToEnd();
		}

		//循环处理
		private static bool bTickOk = true;
		private void Tick(object sender, EventArgs e)
		{
			if (!bTickOk) return;
			bTickOk = false;
			TimerTick.Interval = 1000 * 10;

			try
			{
				//有账号运行中
				if (CurrRunningAccount.Running)
				{
					//运行时间已超过设置时间段
					if (!UtilsAccount.IsItemInRunTime(CurrRunningAccount.currItem))
					{
						//佣兵收菜时间截止，提前收，避免浪费
						if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeMerc &&
							CurrRunningAccount.currItem.MercRule == 7)
						{
							UtilsCom.Log($"佣兵大白菜未成熟，天降大雨，提前收割");
							Reboot(false);
						}
						else
						{
							UtilsCom.Log($"准备停止账号{CurrRunningAccount.EmailShow}挂机(" +
							$"{CurrRunningAccount.currItem.StartTimeHour.ToString("D2")}:{CurrRunningAccount.currItem.StartTimeMin.ToString("D2")}-" +
							$"{CurrRunningAccount.currItem.EndTimeHour.ToString("D2")}:{CurrRunningAccount.currItem.EndTimeMin.ToString("D2")})");
							UtilsProcess.StopHearthstone(CurrRunningAccount,
								NeedMultStone, NeedCloseBattle, ForceCloseBattle);
						}
						CurrRunningAccount.Running = false;
						bTickOk = true;
						return;
					}
					//仍在运行中
					else
					{
						//佣兵收菜模式
						if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeMerc &&
							CurrRunningAccount.currItem.MercRule == 7)
						{
							UtilsXml util = new UtilsXml("Settings/Default/HearthHelper.xml");
							int awake = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + CurrRunningAccount.Email.GetHashCode().ToString(),
								"Awake"
							}));
							bool status = bool.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + CurrRunningAccount.Email.GetHashCode().ToString(),
								"AwakeStatus"
							}));
							int curr = (int)DateTime.Now.Subtract(
								Convert.ToDateTime("2010-1-1 00:00:00")).TotalSeconds;
							//未到收菜时间
							if (curr < awake)
							{
								if (status)
								{
									UtilsCom.Log($"佣兵种菜完成，" +
										$"{(awake - curr) / 60}分{(awake - curr) % 60}秒后准备佣兵收菜");
								}
								else 
								{
									UtilsCom.Log($"佣兵正在种菜，" +
                                        $"{(awake - curr) / 60}分{(awake - curr) % 60}秒后检测是否种完");
								}
								bTickOk = true;
								return;
							}
							//已到收菜时间
							else
							{
								UtilsCom.Log($"佣兵大白菜已成熟，开始启动炉石准备收割");
								Reboot(true);
								bTickOk = true;
								return;
							}
						}
						else
						{
							bool bRunningOK = false;
                            DateTime LatestDateTime = default(DateTime);

							//先检测炉石进程是否存在
							if (UtilsProcess.IsRunning(CurrRunningAccount.StonePid))
							{
								// 检查HsMod配置
								UtilsHsModChange.CheckHsModChange(HsModPort, EnableGMMessageShow, EnableEnemyEmote,
									EnableQuickMode, EnableRankInGameShow, EnableCardState);
								//再检测日志时间
								if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeNormal)
								{
									LatestDateTime = UtilsMonitor.GetNormalFileLatestTime();
								}
								else if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeMerc)
								{
									LatestDateTime = UtilsMonitor.GetMercFileLatestTime();
								}
								else if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeBattle)
								{
									LatestDateTime = UtilsMonitor.GetBaconFileLatestTime();
								}
								int inter = (int)DateTime.Now.Subtract(RunDateTime > LatestDateTime ?
												RunDateTime : LatestDateTime).TotalSeconds;
								if (inter < (CheckInterval * 60))
								{
									if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeNormal)
									{
										UtilsCom.Log($"炉石兄弟{inter / 60}分钟{inter % 60}秒内运行正常");
									}
									else if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeMerc)
									{
										UtilsCom.Log($"佣兵插件{inter / 60}分钟{inter % 60}秒内运行正常");
									}
									else if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeBattle)
									{
										UtilsCom.Log($"酒馆{inter / 60}分钟{inter % 60}秒内运行正常");
									}
									if (NeedPushMessage && NeedPushNormal &&
										(DateTime.Now.ToBinary() / 1000 / 10000 - LastPushNormalDateTime) >
										PushNormalInterval * 3600)
									{
										LastPushNormalDateTime = DateTime.Now.ToBinary() / 1000 / 10000 - 60;
										string result;
										UtilsPush.PostMessage(PushPlusToken, "",
											CurrRunningAccount.Email, "", TodayRebootCnt, RebootCntMax,
											UtilsPush.MSG_TYPE.MSG_NORMAL, out result);
										UtilsCom.Log(result);
									}
									bTickOk = true;
									return;
								}
								else
								{
									if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeNormal)
									{
										UtilsCom.Log($"炉石兄弟{CheckInterval}分钟内无任何操作");
										UtilsCom.Log($"天梯日志最新时间={LatestDateTime.ToLongTimeString()}，" +
											$"准备使用重启大法...");
									}
									else if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeMerc)
									{
										UtilsCom.Log($"佣兵插件{CheckInterval}分钟内无任何操作");
										UtilsCom.Log($"佣兵日志最新时间={LatestDateTime.ToLongTimeString()}，" +
											$"准备使用重启大法...");
									}
									else if (CurrRunningAccount.currItem.Mode == (int)GameMode.ModeBattle)
									{
										UtilsCom.Log($"炉石兄弟{CheckInterval}分钟内无任何操作");
										UtilsCom.Log($"酒馆日志最新时间={LatestDateTime.ToLongTimeString()}，" +
										             $"准备使用重启大法...");
									}
								}
							}
							else
							{
                                UtilsCom.Log($"炉石进程[{CurrRunningAccount.StonePid}]掉线，准备使用重启大法...");
                            }

                            if (!bRunningOK)
							{
								
								TodayRebootCnt++;
								TodayRebootCntStr = TodayRebootCnt.ToString() + "/";
								if (TodayRebootCnt > RebootCntMax)
								{
									UtilsCom.Log($"今日重启次数已达最大值{RebootCntMax}，中控自动停止运行");
									if (NeedPushMessage)
									{
										string result;
										UtilsPush.PostMessage(PushPlusToken, "",
											CurrRunningAccount.Email, "", RebootCntMax, RebootCntMax,
											UtilsPush.MSG_TYPE.MSG_STOP, out result);
										UtilsCom.Log(result);
									}
									UtilsProcess.StopHearthstone(CurrRunningAccount,
										NeedMultStone, NeedCloseBattle, ForceCloseBattle);
									StopRun();
									bTickOk = true;
									return;
								}
								else
								{
									if (NeedPushMessage)
									{
										string result;
										UtilsPush.PostMessage(PushPlusToken, "",
											CurrRunningAccount.Email, "", TodayRebootCnt, RebootCntMax,
											UtilsPush.MSG_TYPE.MSG_REBOOT, out result);
										UtilsCom.Log(result);
									}
									Reboot(true);
									bTickOk = true;
									return;
								}
							}
						}
					}
				}
				//无账号运行中
				else
				{
					if (UtilsAccount.GetFirstInRunTimeItem(IsRunning, ref ForceCloseBattle,
						NeedPushMessage, PushPlusToken, TodayRebootCnt, RebootCntMax,
						AccountList, CurrRunningAccount, out CurrRunningAccount))
					{
						UtilsCom.Log("未检测到目标账号挂机中，准备启动挂机流程...");
						Reboot(true);
					}
					else UtilsCom.Log($"当前时间没有账号需要挂机");
					bTickOk = true;
					return;
				}
			}
			catch
			{
				bTickOk = true;
				return;
			}
		}

		//重启大法
		private void Reboot(bool needHang)
		{
			try
			{
				using (Mutex mut = new Mutex(false, "HearthHelp"))
				{
					try
					{
						mut.WaitOne();

						//获取当前运行时的时间
						RunDateTime = DateTime.Now;

						//停止炉石和战网
						UtilsProcess.StopHearthstone(CurrRunningAccount,
							NeedMultStone, NeedCloseBattle, ForceCloseBattle);
						if (!IsRunning)
						{
							UtilsCom.Log($"用户主动停止运行，终止后续启动");
							return;
						}
						UtilsCom.Delay(5000);

						//切换账号
						UtilsAccount.ChangeLoginAccount(AccountList, CurrRunningAccount.Email);
						UtilsCom.Delay(500);

						//开始启动炉石
						ForceCloseBattle = false;
						bool HearthstoneOk = UtilsProcess.StartHearthstone(
							CurrRunningAccount, HearthstonePath,
							ref IsRunning, NeedMultStone, NeedCloseBattle,
							ForceCloseBattle, needHang, BattleNetPath, BNHSInterval,
							WindowWidth, WindowHeight,
                            EnableHsMod, HsModPort);
						if (!IsRunning)
						{
							UtilsCom.Log($"用户主动停止运行，终止后续启动");
							return;
						}

						//开始启动炉石兄弟
						if (HearthstoneOk)
						{
							UtilsCom.Log($"{HSHBInterval}秒后启动炉石兄弟");
							UtilsCom.Delay(1000 * HSHBInterval);
							UtilsProcess.StartHearthbuddy(CurrRunningAccount,
								ref IsRunning, HearthbuddyPath, SystemVersion,
                                WindowWidth, WindowHeight);
							if (!IsRunning)
							{
								UtilsCom.Log($"用户主动停止运行，终止后续启动");
								return;
							}
							UtilsCom.Log($"炉石兄弟已启动，30秒后开始循环检测");
							UtilsCom.Delay(30 * 1000);
						}

						//下次默认收菜时间，避免炉石启动异常未正确写收菜时间导致异常
						if (HearthstoneOk && CurrRunningAccount.currItem.MercRule == 7 &&
							CurrRunningAccount.currItem.Mode == (int)GameMode.ModeMerc)
						{
							UtilsXml util = new UtilsXml("Settings/Default/HearthHelper.xml");
							TimeSpan ts = DateTime.Now.AddMinutes(CheckInterval).Subtract(
								Convert.ToDateTime("2010-1-1 00:00:00"));
							util.Write(((int)ts.TotalSeconds).ToString(), new string[]
							{
								"BattleNetAccount",
								"HASH" + CurrRunningAccount.Email.GetHashCode().ToString(),
								"Awake"
							});
							util.Write((false).ToString(), new string[]
							{
								"BattleNetAccount",
								"HASH" + CurrRunningAccount.Email.GetHashCode().ToString(),
								"AwakeStatus"
							});
						}

						//启动完成
						if (HearthstoneOk && IsRunning)
						{
							CurrRunningAccount.Running = true;
							StartOrStopButton.Content =
								"停止运行（当前挂机账号" + CurrRunningAccount.EmailShow + "）";
							UtilsCom.Log("预计已全部启动成功，开始循环检测");
						}
					}
					finally
					{
						mut.ReleaseMutex();
					}
				}
			}
			catch (Exception ex)
			{
				UtilsCom.Log(ex.ToString());
			}
		}

		//战网路径选择处理
		private void SelectBattleNetFileButton_Click(object sender, RoutedEventArgs e)
		{
			string text = UtilsPath.SelectExeFile("Battle.net Launcher.exe|*.exe");
			if (!string.IsNullOrEmpty(text))
			{
				BattleNetPath = text;
				UtilsCom.Log("战网路径配置成功：");
				UtilsCom.Log(text);
			}
		}

		//炉石路径选择处理
		private void SelectHearthstoneFileButton_Click(object sender, RoutedEventArgs e)
		{
			string text = UtilsPath.SelectBrowser();
			if (!string.IsNullOrEmpty(text))
			{
				HearthstonePath = text;
				UtilsCom.Log("炉石路径配置成功：");
				UtilsCom.Log(text);
			}
		}

		//兄弟路径选择处理
		private void SelectHearthbuddyFileButton_Click(object sender, RoutedEventArgs e)
		{
			string text = UtilsPath.SelectExeFile("Hearthbuddy.exe|*.exe");
			if (!string.IsNullOrEmpty(text))
			{
				HearthbuddyPath = text;
				UtilsCom.Log("兄弟路径配置成功：");
				UtilsCom.Log(text);
			}
		}

        //进入官网获取Token
        private void GetPushTokenButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("https://www.pushplus.plus/push1.html");
			UtilsCom.Log("IE浏览器扫码后无法跳页面时，请更换其他浏览器重新扫描二维码");
		}

		//测试消息推送
		private void TestPushMessageButton_Click(object sender, RoutedEventArgs e)
		{
			string result;
			UtilsPush.PostMessage(PushPlusToken,
				"123456789@qq.com",
				"987654321@qq.com", "", 5, 30,
				UtilsPush.MSG_TYPE.MSG_TEST, out result);
			UtilsCom.Log(result);
			System.Windows.MessageBox.Show(
				result.ToString(), "", MessageBoxButton.OK);
		}

		//数字输入
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}
		private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = false;
			e.Handled = true;
		}

        //修改账号token
        private void ConfigChangeAccountToken_Click(object sender, RoutedEventArgs e)
        {
	        System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
	        if (button != null)
	        {
		        string account = "";
		        string token = "";
		        AccountPopupChange accountConfigPopup = new AccountPopupChange(AccountList);
		        accountConfigPopup.Left = Left + (Width - accountConfigPopup.Width) / 2.0;
		        accountConfigPopup.Top = Top + (Height - accountConfigPopup.Height) / 2.0;
		        if ((bool)accountConfigPopup.ShowDialog())
		        {
			        account = accountConfigPopup.GetAccount();
			        token = accountConfigPopup.GetToken();
		        }

		        accountConfigPopup.Close();
		        if (!string.IsNullOrEmpty(account))
		        {
			        var accountItemWhole = AccountList.ToList().Find(acc => acc.Email == account);
			        if (accountItemWhole == null)
			        {
				        return;
			        }

			        accountItemWhole.Token = token;
			        if (string.IsNullOrEmpty(token))
			        {
				        UtilsAccount.ChangeLoginAccount(AccountList, CurrRunningAccount.Email);
				        UtilsCom.Delay(500);
				        UtilsProcess.StopHearthstone(CurrRunningAccount, false, true, true);
				        UtilsCom.Delay(2000);
				        UtilsProcess.StartBattleNet(BattleNetPath);
			        }
		        }
	        }
        }

        //账号统计信息查看
        private void ConfigAccountButtonView_Click(object sender, RoutedEventArgs e)
		{
            if (CurrRunningAccount.Running)
			{
				Process.Start(string.Format($"http://localhost:{HsModPort}"));
			}
			else UtilsCom.Log("请先安装和启用HsMod并且运行程序");
		}
 
        //账号添加
        private void ConfigAccountButtonAdd_Click(object sender, RoutedEventArgs e)
		{
			if (AccountList.Count>=5)
			{
				System.Windows.MessageBox.Show("当前账号已达5个，无法再添加", "提示",
					MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
			{
				System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
				if (button != null)
				{
					string newAccount = "";
					string token = "";
					AccountPopupAdd accountConfigPopup = new AccountPopupAdd();
					accountConfigPopup.Left = Left + (Width - accountConfigPopup.Width) / 2.0;
					accountConfigPopup.Top = Top + (Height - accountConfigPopup.Height) / 2.0;
					if ((bool)accountConfigPopup.ShowDialog())
					{
						newAccount = accountConfigPopup.GetAccount();
						token = accountConfigPopup.GetToken();
					}
					accountConfigPopup.Close();
					if (!string.IsNullOrEmpty(newAccount))
					{
						var accountItemWhole = AccountList.ToList().Find(acc => acc.Email == newAccount);
						if (accountItemWhole == null)
						{
							if (UtilsAccount.AddLoginAccount(AccountList, newAccount))
							{
								accountItemWhole = new AccountItemWhole(false, newAccount);
							}
						}

						if (accountItemWhole == null)
						{
							return;
						}

						accountItemWhole.Token = token;
						if (string.IsNullOrEmpty(token))
						{
							UtilsAccount.ChangeLoginAccount(AccountList, CurrRunningAccount.Email);
							UtilsCom.Delay(500);
							UtilsProcess.StopHearthstone(CurrRunningAccount, false, true, true);
							UtilsCom.Delay(2000);
							UtilsProcess.StartBattleNet(BattleNetPath);
						}
						
					}
				}
			}
		}

		//账号配置
		private void ConfigAccountButtonModify_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			if (button != null)
			{
				AccountItemWhole whole = button.Tag as AccountItemWhole;
				int index = AccountList.IndexOf(whole);
				AccountPopupList accountConfigPopup = new AccountPopupList(ref whole);
				accountConfigPopup.Left = Left + (Width - accountConfigPopup.Width) / 2.0;
				accountConfigPopup.Top = Top + (Height - accountConfigPopup.Height) / 2.0;
				accountConfigPopup.ShowDialog();
				AccountList[index] = whole;
			}
		}

		//账号删除
		private void ConfigAccountButtonDelete_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			if (button != null)
			{
				if (System.Windows.MessageBox.Show(this,"确定删除此账号？", "警告",
					MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
				{
					AccountItemWhole whole = button.Tag as AccountItemWhole;
					if (UtilsAccount.DeleteLoginAccount(AccountList, whole.Email))
					{
						AccountList.Remove(whole);
					}
				}
			}
		}

		private void DeleteBepButton_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			if (button != null)
			{
				if (System.Windows.MessageBox.Show(this, "确定卸载HsMod？", "警告",
					MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
				{
					try
					{
						string path = UtilsPath.FindInstallPathFromRegistry("Hearthstone");
						File.Delete(System.IO.Path.Combine(path, "doorstop_config.ini"));
						File.Delete(System.IO.Path.Combine(path, "winhttp.dll"));
						UtilsCopy.DeleteDirectory(System.IO.Path.Combine(path, "BepInEx"));
						UtilsCom.Log("HsMod卸载成功，如需继续使用，请点击“安装HsMod”");
					}
					catch { UtilsCom.Log("HsMod卸载失败，请检查权限"); }
				}
			}
		}

		//插件安装
		private void CopyBepButton_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			if (button != null)
			{
                //复制佣兵插件文件到炉石根目录
                new Thread(new ParameterizedThreadStart(
                	UtilsCopy.CopyMercFileToHearthPath)).Start(HearthstonePath);
                UtilsCom.Log("HsMod安装成功");
            }
		}

        //打炉石补丁
        private void PatchStoneButton0_Click(object sender, RoutedEventArgs e)
        {
            if (UtilsPatch.PatchHearthStone(HearthstonePath, 0))
				UtilsCom.Log("备份Assembly-CSharp.dll和libacsdk_x86.dll成功");
            else UtilsCom.Log("备份Assembly-CSharp.dll和libacsdk_x86.dll失败，请先关闭炉石客户端");
        }
        private void PatchStoneButton1_Click(object sender, RoutedEventArgs e)
        {
            if (UtilsPatch.PatchHearthStone(HearthstonePath, 1)) UtilsCom.Log("打窗口最小化补丁成功");
            else UtilsCom.Log("打窗口最小化补丁失败，请先关闭炉石客户端");
        }
        private void PatchStoneButton2_Click(object sender, RoutedEventArgs e)
        {
            if (UtilsPatch.PatchHearthStone(HearthstonePath, 2)) UtilsCom.Log("打反作弊补丁成功");
            else UtilsCom.Log("打反作弊补丁失败，请先关闭炉石客户端");
        }
        private void PatchStoneButton3_Click(object sender, RoutedEventArgs e)
        {
            if (UtilsPatch.PatchHearthStone(HearthstonePath, 3)) UtilsCom.Log("打去除广告补丁成功");
            else UtilsCom.Log("打去除广告补丁失败，请先关闭炉石客户端");
        }
        private void PatchStoneButton4_Click(object sender, RoutedEventArgs e)
        {
            if (UtilsPatch.PatchHearthStone(HearthstonePath, 4)) UtilsCom.Log("打去除特有开门补丁成功");
            else UtilsCom.Log("打去除特有开门补丁失败，请先关闭炉石客户端");
        }
        private void PatchStoneButton5_Click(object sender, RoutedEventArgs e)
        {
            if (UtilsPatch.PatchHearthStone(HearthstonePath, 5)) UtilsCom.Log("打去除金卡特效补丁成功");
            else UtilsCom.Log("打去除金卡特效补丁失败，请先关闭炉石客户端");
        }
        private void PatchStoneButton6_Click(object sender, RoutedEventArgs e)
        {
            if (UtilsPatch.PatchHearthStone(HearthstonePath, 6))
				UtilsCom.Log("还原Assembly-CSharp.dll和libacsdk_x86.dll成功");
            else UtilsCom.Log("还原Assembly-CSharp.dll和libacsdk_x86.dll失败，请先关闭炉石客户端");
        }

        private void ConfigSave()
		{
            //写入配置文件
            UtilsConfig.WriteConfig(AccountList,
	            BattleNetPath, HearthstonePath,
	            HearthbuddyPath, PushPlusToken,
	            BNHSInterval, HSHBInterval, CheckInterval,
	            RebootCntMax, PushNormalInterval, SystemVersion,
	            WindowWidth, WindowHeight,
	            NeedCloseBattle, NeedMultStone,
	            NeedPushMessage, NeedPushNormal,
	            EnableHsMod, HsModPort,
	            EnableGMMessageShow, EnableEnemyEmote,
	            EnableQuickMode, EnableRankInGameShow, EnableCardState);

            //HsMod插件
            /*
            if (!EnableHsMod)
            {
                try
                {
                    string path = UtilsPath.FindInstallPathFromRegistry("Hearthstone");
                    File.Delete(System.IO.Path.Combine(path, "doorstop_config.ini"));
                    File.Delete(System.IO.Path.Combine(path, "winhttp.dll"));
                    UtilsCopy.DeleteDirectory(System.IO.Path.Combine(path, "BepInEx"));
                    UtilsCopy.DeleteDirectory(System.IO.Path.Combine(path, "unstripped_corlib"));
                    UtilsCom.Log("插件卸载成功，如需继续使用，请点击“安装插件”");
                }
                catch { UtilsCom.Log("插件卸载失败，请检查权限"); }
            }
            else
            {
                //复制佣兵插件文件到炉石根目录
                new Thread(new ParameterizedThreadStart(
                    UtilsCopy.CopyMercFileToHearthPath)).Start(HearthstonePath);
                UtilsCom.Log("插件安装成功");

                //清空命令行参数，禁用插件启用
                string path = UtilsPath.GetBattleConfig();
                string json = File.ReadAllText(path);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                jsonObj["Games"]["hs_beta"]["AdditionalLaunchArguments"] = "";
                File.WriteAllText(path, JsonConvert.SerializeObject(
                    jsonObj, Newtonsoft.Json.Formatting.Indented));
                var MyIni = new UtilsIniFile(System.IO.Path.Combine(
                    HearthstonePath, "doorstop_config.ini"));
                MyIni.Write("enabled", "false", "UnityDoorstop");

                //删除HsMod配置，以防出问题
                //try
                //{
                //	string path1 = UtilsPath.FindInstallPathFromRegistry("Hearthstone");
                //	File.Delete(Path.Combine(path1,@"BepInEx\config\HsMod.cfg"));
                //}
                //catch{ }
            }*/
        }

        private void Timer_Tick1(object sender, EventArgs e)
        {
            elapsedTime += TimeSpan.FromSeconds(1);
			timeTextBlock1.Text = string.Format("运行时间：{0}",
				elapsedTime.ToString("d'天 'h'小时 'm'分 's'秒'"));
        }

        private void Timer_Tick2(object sender, EventArgs e)
        {
            timeTextBlock2.Text = string.Format("{0}",
                DateTime.Now.ToString("g"));
        }

        private void WindowExit()
		{
            ConfigSave();
            Environment.Exit(0);
        }

        //窗口加载
        private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Title = MyTitlt;
			UtilsCom.Log("请先完整配置一次炉石兄弟运行成功后，再使用中控");
			UtilsCom.Log("同时必须先在中控里面配置账号的模式、卡组、策略");

            //禁止按钮
            DisableButton();
            StartOrStopButton.IsEnabled = false;

            //删除旧的升级文件
            string rootPath = Directory.GetCurrentDirectory() + "\\";
            string prefix = "Update_HearthScript";
            string[] files = Directory.GetFiles(rootPath,prefix+"*");
            foreach (var file in files)
            {
                try
                {
                    UtilsCom.Log($"删除升级包[{file.Substring(file.LastIndexOf(prefix))}]");
                    File.Delete(file);
                }
                catch { }
            }

            //读取配置文件
            ObservableCollection<AccountItemWhole> _AccountList =
				new ObservableCollection<AccountItemWhole>();
			string _BattleNetPath = ""; string _HearthstonePath = "";
			string _HearthbuddyPath = ""; string _PushPlusToken = "";
			int _BNHSInterval = 0; int _HSHBInterval = 0; int _CheckInterval = 0;
			int _RebootCntMax = 0; int _PushNormalInterval = 0; int _SystemVersion = 0;
			int _WindowWidth = 144; int _WindowHeight = 108;
			bool _NeedCloseBattle = false; bool _NeedMultStone = false;
			bool _NeedPushMessage = false; bool _NeedPushNormal = false;
			bool _EnableHsMod = false; int _HsModPort = 58744;
			bool _EnableGMMessageShow = false; bool _EnableEnemyEmote= false;
			bool _EnableQuickMode = false; bool _EnableRankInGameShow= false;
			bool _EnableCardState = false;
            UtilsConfig.ReadConfig(ref _AccountList,
				ref _BattleNetPath, ref _HearthstonePath,
				ref _HearthbuddyPath, ref _PushPlusToken,
				ref _BNHSInterval, ref _HSHBInterval, ref _CheckInterval,
				ref _RebootCntMax, ref _PushNormalInterval, ref _SystemVersion,
				ref _WindowWidth, ref _WindowHeight,
				ref _NeedCloseBattle, ref _NeedMultStone,
				ref _NeedPushMessage, ref _NeedPushNormal,
                ref _EnableHsMod, ref _HsModPort,
                ref _EnableGMMessageShow, ref _EnableEnemyEmote,
                ref _EnableQuickMode, ref _EnableRankInGameShow,
                ref _EnableCardState);
			AccountList = _AccountList;
			BattleNetPath = _BattleNetPath; HearthstonePath = _HearthstonePath;
			HearthbuddyPath = _HearthbuddyPath; PushPlusToken = _PushPlusToken;
			BNHSInterval = _BNHSInterval; HSHBInterval = _HSHBInterval;
			CheckInterval = _CheckInterval;
			RebootCntMax = _RebootCntMax; PushNormalInterval = _PushNormalInterval;
			SystemVersion = _SystemVersion;
			WindowWidth = _WindowWidth; WindowHeight = _WindowHeight;
			NeedCloseBattle = _NeedCloseBattle; NeedMultStone = _NeedMultStone;
			NeedPushMessage = _NeedPushMessage; NeedPushNormal = _NeedPushNormal;
            HsModPort = _HsModPort;
            EnableGMMessageShow = _EnableGMMessageShow; EnableEnemyEmote = _EnableEnemyEmote;
            EnableQuickMode = _EnableQuickMode; EnableRankInGameShow = _EnableRankInGameShow;
            EnableCardState = _EnableCardState;
            AccountListBox.ItemsSource = AccountList;

			//检测日志路径
			UtilsMonitor.CheckLogPath();

            //定时器
            TimerLog.Interval = 100;
            TimerLog.Tick += ShowLog;
            TimerLog.Start();
            TimerMonitor.Interval = 1000;
            TimerMonitor.Tick += MyMonitor;
            TimerMonitor.Start();
            // TimerInterval.Interval = checkUpdateInterval;
            // TimerInterval.Tick += MyUpdate;
            // TimerInterval.Start();
            TimerTick.Interval = 1000;
            TimerTick.Tick += Tick;
            TimerInit.Interval = 10;
            TimerInit.Tick += Init;
            TimerInit.Start();
            timerRun1 = new DispatcherTimer();
            timerRun1.Interval = TimeSpan.FromSeconds(1);
            timerRun1.Tick += Timer_Tick1;
            timerRun1.Start();
            timerRun2 = new DispatcherTimer();
            timerRun2.Interval = TimeSpan.FromSeconds(1);
            timerRun2.Tick += Timer_Tick2;
            timerRun2.Start();
        }

		//窗口关闭
		private void Window_Closed(object sender, EventArgs e)
		{
			WindowExit();
        }

        private void Init(object sender, EventArgs e)
		{
            TimerInit.Stop();

            //首次校验
            // IntervalCheck();

            //使能按钮
            EnableButton();
            StartOrStopButton.IsEnabled = true;
        }

		private void DisableButton()
		{
            SelectBattleNetFileButton.IsEnabled = false;
            SelectHearthstoneFileButton.IsEnabled = false;
            SelectHearthbuddyFileButton.IsEnabled = false;
            GetPushTokenButton.IsEnabled = false;
            TestPushMessageButton.IsEnabled = false;
            AccountListBox.IsEnabled = false;
            ConfigAccountButtonAdd.IsEnabled = false;
            BattleNetPathTextBox.IsEnabled = false;
            HearthstonePathTextBox.IsEnabled = false;
            HearthbuddyPathTextBox.IsEnabled = false;
            PushPlusTokenTextBox.IsEnabled = false;
            BNHSIntervalTextBox.IsEnabled = false;
            HSHBIntervalTextBox.IsEnabled = false;
            CheckIntervalTextBox.IsEnabled = false;
            RebootMaxCntTextBox.IsEnabled = false;
            SystemVersionComboBox.IsEnabled = false;
            WindowWidthTextBox.IsEnabled = false;
            WindowHeightTextBox.IsEnabled = false;
            PushNormalIntervalTextBox.IsEnabled = false;
            NeedCloseBattleCheckBox.IsEnabled = false;
            NeedMultStoneCheckBox.IsEnabled = false;
            NeedPushMessageCheckBox.IsEnabled = false;
            NeedPushNormalCheckBox.IsEnabled = false;
            AddAccountButton.IsEnabled = false;
            /*
            EnableHsModCheckBox.IsEnabled = false;
            HsModPortTextBox.IsEnabled = false;
            CopyBepButton.IsEnabled = false;
            DeleteBepButton.IsEnabled = false;
            EnableTimeGearCheckBox.IsEnabled = false;
            NoFightTimeTextBox.IsEnabled = false;
            PveFightTimeTextBox.IsEnabled = false;
            PvpFightTimeTextBox.IsEnabled = false;
			if (!EnableHsMod)
			{
                ConfigAccountButtonView.IsEnabled = false;
            }
			*/
        }

        private void EnableButton()
        {
            SelectBattleNetFileButton.IsEnabled = true;
            SelectHearthstoneFileButton.IsEnabled = true;
            //SelectHearthbuddyFileButton.IsEnabled = true;
            GetPushTokenButton.IsEnabled = true;
            TestPushMessageButton.IsEnabled = true;
            AccountListBox.IsEnabled = true;
            ConfigAccountButtonAdd.IsEnabled = true;
            BattleNetPathTextBox.IsEnabled = true;
            HearthstonePathTextBox.IsEnabled = true;
            //HearthbuddyPathTextBox.IsEnabled = true;
            PushPlusTokenTextBox.IsEnabled = true;
            BNHSIntervalTextBox.IsEnabled = true;
            HSHBIntervalTextBox.IsEnabled = true;
            CheckIntervalTextBox.IsEnabled = true;
            RebootMaxCntTextBox.IsEnabled = true;
            SystemVersionComboBox.IsEnabled = true;
            WindowWidthTextBox.IsEnabled = true;
            WindowHeightTextBox.IsEnabled = true;
            PushNormalIntervalTextBox.IsEnabled = true;
            NeedCloseBattleCheckBox.IsEnabled = true;
            NeedMultStoneCheckBox.IsEnabled = true;
            NeedPushMessageCheckBox.IsEnabled = true;
            NeedPushNormalCheckBox.IsEnabled = true;
            AddAccountButton.IsEnabled = true;
            /*
			EnableHsModCheckBox.IsEnabled = true;
            HsModPortTextBox.IsEnabled = true;
            CopyBepButton.IsEnabled = true;
            DeleteBepButton.IsEnabled = true;
            EnableTimeGearCheckBox.IsEnabled = true;
            NoFightTimeTextBox.IsEnabled = true;
            PveFightTimeTextBox.IsEnabled = true;
            PvpFightTimeTextBox.IsEnabled = true;
            ConfigAccountButtonView.IsEnabled = true;
			*/
        }

        //开始运行
        private void StartRun()
		{
			if (!IsRunning)
			{
				//配置错误
				bool bError = false;
				if (!File.Exists(BattleNetPath))
				{
					bError = true;
					UtilsCom.Log("战网路径配置错误");
				}
                if (bError) return;
                if (!File.Exists(HearthbuddyPath))
				{
					bError = true;
					UtilsCom.Log("兄弟路径配置错误，请勿将本程序单独放置");
				}
                if (bError) return;
                if (!bError && AppDomain.CurrentDomain.BaseDirectory !=
					Directory.GetParent(HearthbuddyPath).FullName + "\\")
				{
					bError = true;
					UtilsCom.Log("本程序未放置在兄弟根目录");
					UtilsCom.Log("当前目录：" + AppDomain.CurrentDomain.BaseDirectory);
					UtilsCom.Log("所配置兄弟目录：" + Directory.GetParent(HearthbuddyPath).FullName + "\\");
				}
                if (bError) return;
                if (UtilsCom.IsHasCHZN(HearthbuddyPath))
				{
                    bError = true;
                    UtilsCom.Log("本程序禁止解压到中文目录下");
                }
                if (bError) return;
                if (AppDomain.CurrentDomain.BaseDirectory.Contains(HearthstonePath))
				{
                    bError = true;
                    UtilsCom.Log("本程序禁止解压到炉石目录或其子目录");
                    UtilsCom.Log("请解压到桌面纯英文目录下");
                }
				if (bError) return;

                //保存配置文件
                ConfigSave();
                
                // 比对HsMod
                UtilsCopy.CopyMercFileToHearthPath(HearthstonePath);
                UtilsCom.Log("HsMod安装成功");

                //开始运行
                IsRunning = true;
				TimerTick.Interval = 1000;
				TimerTick.Start();
				StartOrStopButton.Content = "停止运行";
				DisableButton();
                UtilsCom.Log($"配置成功，兄弟中控开始运行...");
				if(NeedMultStone) UtilsCom.Log("多开模式会一直在等待其他中控操作完成，请稍等...");
			}
		}

		private void StopRun()
		{
			if (IsRunning)
			{
				//停止运行
				IsRunning = false;
				TimerTick.Interval = 1000;
				TimerTick.Stop();
				CurrRunningAccount.Running = false;
				StartOrStopButton.Content = "开始运行";
				EnableButton();
                UtilsCom.Log($"兄弟中控已停止");
			}
		}

		//开始或停止
		private void StartOrStopButton_Click(object sender, RoutedEventArgs e)
		{
			if (IsRunning) StopRun();
			else StartRun();
		}
	}
}

