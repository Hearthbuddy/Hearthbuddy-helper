using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace HearthHelper
{
	internal class UtilsConfig
	{
		public const int DefaultBNHSInterval = 20;
		public const int DefaultHSHBInterval = 30;
		public const int DefaultCheckInterval = 5;
		public const int DefaultRebootCntMax = 30;
		public const int DefaultSystemVersion = 0;
		public const int DefaultWindowWidth = 144;
		public const int DefaultWindowHeight = 108;
		public const int DefaultPushNormalInterval = 2;
        public const int DefaultHsModPort = 58744;
        public const int DefaultNoFightTime = 2;
        public const int DefaultPveFightTime = 4;
        public const int DefaultPvpFightTime = 1;

        public static void ReadConfig(
			ref ObservableCollection<AccountItemWhole> AccountList,
			ref string BattleNetPath, ref string HearthstonePath,
			ref string HearthbuddyPath, ref string PushPlusToken,
			ref int BNHSInterval, ref int HSHBInterval, ref int CheckInterval,
			ref int RebootCntMax, ref int PushNormalInterval, ref int SystemVersion,
			ref int WindowWidth, ref int WindowHeight,
			ref bool NeedCloseBattle, ref bool NeedMultStone,
			ref bool NeedPushMessage,ref bool NeedPushNormal,
            ref bool EnableHsMod, ref int HsModPort,
			ref bool EnableTimeGear, ref int NoFightTime,
			ref int PveFightTime, ref int PvpFightTime)
		{
			//加载xml配置文件
			UtilsXml util = new UtilsXml("Settings/Default/HearthHelper.xml");

			//路径
			BattleNetPath = util.Read("BattleNetPath");
			HearthstonePath = util.Read("HearthstonePath");
            HearthbuddyPath = util.Read("HearthbuddyPath");
            PushPlusToken = util.Read("PushPlusToken");
			if (string.IsNullOrEmpty(BattleNetPath) ||
                !BattleNetPath.Contains("Battle.net"))
			{
				BattleNetPath = UtilsPath.AutoGetBattleNetPath();
			}
            if (string.IsNullOrEmpty(HearthstonePath) ||
                !HearthstonePath.Contains("Hearthstone") ||
                HearthstonePath.Contains("Assembly-CSharp.dll"))
            {
                HearthstonePath = UtilsPath.AutoGetHearthstonePath();
            }
            if (string.IsNullOrEmpty(HearthbuddyPath) ||
                !HearthbuddyPath.Contains(AppDomain.CurrentDomain.BaseDirectory))
            {
                HearthbuddyPath = UtilsPath.AutoGetHearthBuddyPath();
            }

            //读取数据
            try { BNHSInterval = int.Parse(util.Read("BNHSInterval")); }
			catch
			{
				BNHSInterval = DefaultBNHSInterval;
				UtilsCom.Log($"读取数据错误，恢复默认值={DefaultBNHSInterval}");
			}
			try { HSHBInterval = int.Parse(util.Read("HSHBInterval")); }
			catch
			{
				HSHBInterval = DefaultHSHBInterval;
				UtilsCom.Log($"读取数据错误，恢复默认值={DefaultHSHBInterval}");
			}
			try { CheckInterval = int.Parse(util.Read("CheckInterval")); }
			catch
			{
				CheckInterval = DefaultCheckInterval;
				UtilsCom.Log($"读取数据错误，恢复默认值={DefaultCheckInterval}");
			}
			try { RebootCntMax = int.Parse(util.Read("RebootCntMax")); }
			catch
			{
				RebootCntMax = DefaultRebootCntMax;
				UtilsCom.Log($"读取数据错误，恢复默认值={DefaultRebootCntMax}");
			}
			try { PushNormalInterval = int.Parse(util.Read("PushNormalInterval")); }
			catch
			{
				PushNormalInterval = DefaultPushNormalInterval;
				UtilsCom.Log($"读取数据错误，恢复默认值={DefaultPushNormalInterval}");
			}
			try { NeedCloseBattle = bool.Parse(util.Read("NeedCloseBattle")); }
			catch
			{
				NeedCloseBattle = true;
				UtilsCom.Log($"读取数据错误，恢复默认值={true}");
			}
			try { NeedMultStone = bool.Parse(util.Read("NeedMultStone")); }
			catch
			{
				NeedMultStone = false;
				UtilsCom.Log($"读取数据错误，恢复默认值={false}");
			}
			try { NeedPushMessage = bool.Parse(util.Read("NeedPushMessage")); }
			catch
			{
				NeedPushMessage = true;
				UtilsCom.Log($"读取数据错误，恢复默认值={true}");
			}
			try { NeedPushNormal = bool.Parse(util.Read("NeedPushNormal")); }
			catch
			{
				NeedPushNormal = false;
				UtilsCom.Log($"读取数据错误，恢复默认值={false}");
			}
			try { SystemVersion = int.Parse(util.Read("SystemVersion")); }
			catch
			{
				SystemVersion = DefaultSystemVersion;
				UtilsCom.Log($"读取数据错误，恢复默认值={DefaultSystemVersion}");
			}
			try { WindowWidth = int.Parse(util.Read("WindowWidth")); }
			catch
			{
				WindowWidth = DefaultWindowWidth;
				UtilsCom.Log($"读取数据错误，恢复默认值={DefaultWindowWidth}");
			}
			try { WindowHeight = int.Parse(util.Read("WindowHeight")); }
			catch
			{
				WindowHeight = DefaultWindowHeight;
				UtilsCom.Log($"读取数据错误，恢复默认值={DefaultWindowHeight}");
			}
            try { EnableHsMod = bool.Parse(util.Read("EnableHsMod")); }
            catch
            {
                EnableHsMod = false;
                UtilsCom.Log($"读取数据错误，恢复默认值={false}");
            }
            try { HsModPort = int.Parse(util.Read("HsModPort")); }
            catch
            {
                HsModPort = DefaultHsModPort;
                UtilsCom.Log($"读取数据错误，恢复默认值={DefaultHsModPort}");
            }
            try { EnableTimeGear = bool.Parse(util.Read("EnableTimeGear")); }
            catch
            {
                EnableTimeGear = false;
                UtilsCom.Log($"读取数据错误，恢复默认值={false}");
            }
            try { NoFightTime = int.Parse(util.Read("NoFightTime")); }
            catch
            {
                NoFightTime = DefaultNoFightTime;
                UtilsCom.Log($"读取数据错误，恢复默认值={DefaultNoFightTime}");
            }
            try { PveFightTime = int.Parse(util.Read("PveFightTime")); }
            catch
            {
                PveFightTime = DefaultPveFightTime;
                UtilsCom.Log($"读取数据错误，恢复默认值={DefaultPveFightTime}");
            }
            try { PvpFightTime = int.Parse(util.Read("PvpFightTime")); }
            catch
            {
                PvpFightTime = DefaultPvpFightTime;
                UtilsCom.Log($"读取数据错误，恢复默认值={DefaultPvpFightTime}");
            }
            //读取战网账号配置
            string path = UtilsPath.GetBattleConfig();
			try
			{
				using (StreamReader file = File.OpenText(path))
				{
					using (JsonTextReader reader = new JsonTextReader(file))
					{
						//读取
						JObject jsonObj = (JObject)JToken.ReadFrom(reader);
						string saved = (string)jsonObj["Client"]["SavedAccountNames"];
						string[] sArray = Regex.Split(saved, ",", RegexOptions.IgnoreCase);
						foreach (string account in sArray)
						{
							AccountList.Add(new AccountItemWhole(false, account));
						}
						file.Close();
					}
				}
			}
			catch (Exception ex)
			{
				UtilsCom.Log(ex.Message);
				UtilsCom.Log("无法读取战网用户数据");
			}

			//读取用户配置数据
			foreach (AccountItemWhole accountCheckedItem in AccountList)
			{
				try
				{
					accountCheckedItem.Selected = bool.Parse(util.Read(new string[]
					{
						"BattleNetAccount",
						"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
						"Selected"
					}));
					int i = 0;
					for ( ; ; )
					{
						string item = string.Format("AccountItem{0}", i++);
						if (null != util.Read(new string[]{"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),item}))
						{
							AccountItemSingle single = new AccountItemSingle(0);
							single.Mode = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"Mode"
							}));
							single.Enable = bool.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"Enable"
							}));
							single.StartTimeHour = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"StartTimeHour"
							}));
							single.StartTimeMin = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"StartTimeMin"
							}));
							single.EndTimeHour = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"EndTimeHour"
							}));
							single.EndTimeMin = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"EndTimeMin"
							}));
							single.NormalRule = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"NormalRule"
							}));
							single.NormalBehavior = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"NormalBehavior"
							}));
							single.NormalDeck = util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"NormalDeck"
							});
							single.MercRule = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"MercRule"
							}));
							single.MercBehavior = int.Parse(util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"MercBehavior"
							}));
							single.MercTeam = util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"MercTeam"
							});
							single.MercMap = util.Read(new string[]
							{
								"BattleNetAccount",
								"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
								item,
								"MercMap"
							});
							try
							{
								single.MercInterval = int.Parse(util.Read(new string[]
								{
									"BattleNetAccount",
									"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
									item,
									"MercInterval"
								}));
							}
							catch { single.MercInterval = 22; }
							try
							{
								single.MercConcede = bool.Parse(util.Read(new string[]
								{
									"BattleNetAccount",
									"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
									item,
									"MercConcede"
								}));
							}
							catch { single.MercConcede = false; }
							try
							{
								single.MercCraft = bool.Parse(util.Read(new string[]
								{
									"BattleNetAccount",
									"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
									item,
									"MercCraft"
								}));
							}
							catch { single.MercCraft = true; }
							try
							{
								single.MercUpdate = bool.Parse(util.Read(new string[]
								{
									"BattleNetAccount",
									"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
									item,
									"MercUpdate"
								}));
							}
							catch { single.MercUpdate = false; }
							accountCheckedItem.itemList.Add(single);
						}
						else break;
					}
				}
				catch
				{
					UtilsCom.Log($"读取账号{accountCheckedItem.EmailShow}数据错误");
				}
			}
		}

		public static void WriteConfig(
			ObservableCollection<AccountItemWhole> AccountList,
			string BattleNetPath, string HearthstonePath,
			string HearthbuddyPath,string PushPlusToken,
			int BNHSInterval, int HSHBInterval, int CheckInterval,
			int RebootCntMax, int PushNormalInterval, int SystemVersion,
			int WindowWidth, int WindowHeight,
			bool NeedCloseBattle, bool NeedMultStone,
			bool NeedPushMessage, bool NeedPushNormal,
            bool EnableHsMod, int HsModPort,
            bool EnableTimeGear, int NoFightTime,
            int PveFightTime, int PvpFightTime)
		{
			try
			{
				File.Delete("Settings/Default/HearthHelper.xml");
				UtilsXml util = new UtilsXml("Settings/Default/HearthHelper.xml");
				util.Write(BattleNetPath, "BattleNetPath");
				util.Write(HearthstonePath, "HearthstonePath");
				util.Write(HearthbuddyPath, "HearthbuddyPath");
				util.Write(PushPlusToken, "PushPlusToken");
				util.Write(BNHSInterval.ToString(), "BNHSInterval");
				util.Write(HSHBInterval.ToString(), "HSHBInterval");
				util.Write(CheckInterval.ToString(), "CheckInterval");
				util.Write(RebootCntMax.ToString(), "RebootCntMax");
				util.Write(SystemVersion.ToString(), "SystemVersion");
				util.Write(WindowWidth.ToString(), "WindowWidth");
				util.Write(WindowHeight.ToString(), "WindowHeight");
				util.Write(PushNormalInterval.ToString(), "PushNormalInterval");
				util.Write(NeedCloseBattle.ToString(), "NeedCloseBattle");
				util.Write(NeedMultStone.ToString(), "NeedMultStone");
				util.Write(NeedPushMessage.ToString(), "NeedPushMessage");
				util.Write(NeedPushNormal.ToString(), "NeedPushNormal");
                util.Write(EnableHsMod.ToString(), "EnableHsMod");
                util.Write(HsModPort.ToString(), "HsModPort");
                util.Write(EnableTimeGear.ToString(), "EnableTimeGear");
                util.Write(NoFightTime.ToString(), "NoFightTime");
                util.Write(PveFightTime.ToString(), "PveFightTime");
                util.Write(PvpFightTime.ToString(), "PvpFightTime");
				foreach (AccountItemWhole accountCheckedItem in AccountList)
				{
					util.Write(accountCheckedItem.Selected.ToString(), new string[]
					{
						"BattleNetAccount",
						"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
						"Selected"
					});
					int i = 0;
					foreach (AccountItemSingle single in accountCheckedItem.itemList)
					{
						string item = string.Format("AccountItem{0}", i++);
						util.Write(single.Mode.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"Mode"
						});
						util.Write(single.Enable.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"Enable"
						});
						util.Write(single.StartTimeHour.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"StartTimeHour"
						});
						util.Write(single.StartTimeMin.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"StartTimeMin"
						});
						util.Write(single.EndTimeHour.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"EndTimeHour"
						});
						util.Write(single.EndTimeMin.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"EndTimeMin"
						});
						util.Write(single.NormalRule.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"NormalRule"
						});
						util.Write(single.NormalBehavior.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"NormalBehavior"
						});
						util.Write(single.NormalDeck, new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"NormalDeck"
						});
						util.Write(single.MercRule.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"MercRule"
						});
						util.Write(single.MercBehavior.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"MercBehavior"
						});
						util.Write(single.MercTeam, new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"MercTeam"
						});
						util.Write(single.MercMap, new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"MercMap"
						});
						util.Write(single.MercInterval.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"MercInterval"
						});
						util.Write(single.MercConcede.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"MercConcede"
						});
						util.Write(single.MercCraft.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"MercCraft"
						});
						util.Write(single.MercUpdate.ToString(), new string[]
						{
							"BattleNetAccount",
							"HASH" + accountCheckedItem.Email.GetHashCode().ToString(),
							item,
							"MercUpdate"
						});
					}
				}
			}
			catch { }
		}
	}
}

