using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace HearthHelper
{
	internal class UtilsAccount
	{
		//判断当前账号是否满足挂机时间
		public static bool IsItemInRunTime(AccountItemSingle item)
		{
			int sHour = item.StartTimeHour;
			int sMin = item.StartTimeMin;
			int eHour = item.EndTimeHour;
			int eMin = item.EndTimeMin;
			int cHour = DateTime.Now.Hour;
			int cMin = DateTime.Now.Minute;
			bool bSwap = false;
			if (sHour > eHour || (sHour == eHour && sMin > eMin))
			{
				bSwap = true;
				int tmp = sHour;
				sHour = eHour;
				eHour = tmp;
				tmp = sMin;
				sMin = eMin;
				eMin = tmp;
			}
			bool bIn = ((cHour > sHour || (cHour == sHour && cMin >= sMin)) &&
						(cHour < eHour || (cHour == eHour && cMin <= eMin)));
			return item.Enable?(bSwap ? !bIn: bIn):false;
		}

		//添加新账号
		public static bool AddLoginAccount(
			ObservableCollection<AccountItemWhole> AccountList, string email)
		{
			string path = UtilsPath.GetBattleConfig();
			try
			{
				//新账号最开头
				string newInfo = "";
				newInfo += email;
				foreach (AccountItemWhole account in AccountList)
				{
					newInfo += "," + account.Email;
				}

				//写入新信息
				string json = File.ReadAllText(path);
				dynamic jsonObj = JsonConvert.DeserializeObject(json);
				jsonObj["Client"]["SavedAccountNames"] = newInfo;
				string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
				File.WriteAllText(path, output);
				UtilsCom.Log($"添加战网新用户{UtilsCom.ReplaceWithSpecialChar(email)}");
				return true;
			}
			catch (Exception ex)
			{
				UtilsCom.Log(ex.Message);
				UtilsCom.Log("无法加战战网用户数据");
				return false;
			}
		}

		//删除账号
		public static bool DeleteLoginAccount(
			ObservableCollection<AccountItemWhole> AccountList, string email)
		{
			string path = UtilsPath.GetBattleConfig();
			try
			{
				string newInfo = "";
				foreach (AccountItemWhole account in AccountList)
				{
					if (account.Email != email)
						newInfo += (string.IsNullOrEmpty(newInfo) ?
							account.Email:("," + account.Email));
				}

				//写入新信息
				string json = File.ReadAllText(path);
				dynamic jsonObj = JsonConvert.DeserializeObject(json);
				jsonObj["Client"]["SavedAccountNames"] = newInfo;
				string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
				File.WriteAllText(path, output);
				UtilsCom.Log($"删除战网用户{UtilsCom.ReplaceWithSpecialChar(email)}成功");
				return true;
			}
			catch (Exception ex)
			{
				UtilsCom.Log(ex.Message);
				UtilsCom.Log("无法删除战网用户数据");
				return false;
			}
		}

		//修改登录账号
		public static bool ChangeLoginAccount(
			ObservableCollection<AccountItemWhole> AccountList, string email)
		{
			string path = UtilsPath.GetBattleConfig();
			try
			{
				//账号重新排序
				string newInfo = "";
				newInfo += email;
				foreach (AccountItemWhole account in AccountList)
				{
					if (account.Email != email) newInfo += "," + account.Email;
				}

				//写入新信息
				string json = File.ReadAllText(path);
				dynamic jsonObj = JsonConvert.DeserializeObject(json);
				jsonObj["Client"]["SavedAccountNames"] = newInfo;
				string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
				File.WriteAllText(path, output);
				UtilsCom.Log($"修改当前战网用户为{UtilsCom.ReplaceWithSpecialChar(email)}成功");
				return true;
			}
			catch (Exception ex)
			{
				UtilsCom.Log(ex.Message);
				UtilsCom.Log("无法修改战网用户数据");
				return false;
			}
		}

		//获取选中且满足挂机时间的第一个账号
		public static bool GetFirstInRunTimeItem(
			bool IsRunning,ref bool ForceCloseBattle, bool NeedPushMessage,
			string PushPlusToken, int TodayRebootCnt, int RebootCntMax,
			ObservableCollection<AccountItemWhole> AccountList,
			AccountItemWhole CurrRunningAccount, out AccountItemWhole account)
		{
			foreach (AccountItemWhole accountCheckedItem in AccountList)
			{
				foreach (AccountItemSingle item in accountCheckedItem.itemList)
				{
					if (accountCheckedItem.Selected && IsRunning &&
						IsItemInRunTime(item))
					{
						if ((CurrRunningAccount.Email.Length > 0 &&
							!string.Equals(CurrRunningAccount.Email, accountCheckedItem.Email)) ||
							(CurrRunningAccount.currItem!=null &&
							(CurrRunningAccount.currItem.Mode != item.Mode ||
							CurrRunningAccount.currItem.NormalRule != item.NormalRule ||
							CurrRunningAccount.currItem.MercRule != item.MercRule)))
						{
							ForceCloseBattle = true;
							if (NeedPushMessage)
							{
								string result;
								UtilsPush.PostMessage(PushPlusToken, CurrRunningAccount.Email,
									accountCheckedItem.Email, item.Mode == 0 ?
									StringConst.rule0[item.NormalRule] : StringConst.rule1[item.MercRule],
									TodayRebootCnt, RebootCntMax,
									UtilsPush.MSG_TYPE.MSG_CHANGE, out result);
								UtilsCom.Log(result);
							}
						}
						account = accountCheckedItem;
						account.currItem = item;
						UtilsCom.Log($"账号{account.EmailShow}满足挂机时间");
						if (item.Mode == (int)GameMode.ModeNormal)
						{
							UtilsCom.Log($"天梯时间(" + 
								$"{item.StartTimeHour.ToString("D2")}:{item.StartTimeMin.ToString("D2")}-" +
                                $"{item.EndTimeHour.ToString("D2")}:{item.EndTimeMin.ToString("D2")})");
						}
						else if (item.Mode == (int)GameMode.ModeMerc)
						{
							UtilsCom.Log($"佣兵时间(" +
								$"{item.StartTimeHour.ToString("D2")}:{item.StartTimeMin.ToString("D2")}-" +
								$"{item.EndTimeHour.ToString("D2")}:{item.EndTimeMin.ToString("D2")})");
						}
						return true;
					}
				}
			}
			account = new AccountItemWhole(false, "");
			account.currItem = null;
			return false;
		}
	}
}
