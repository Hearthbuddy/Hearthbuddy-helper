using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HearthHelper
{
	public class UtilsPush
	{
		//读取当前账号信息
		private static string ReadCurrAccount(string path)
		{
			string result = "";
			if (!File.Exists(path)) return result;

			using (StreamReader file = File.OpenText(path))
			{
				using (JsonTextReader reader = new JsonTextReader(file))
				{
					//读取
					JObject jsonObject = (JObject)JToken.ReadFrom(reader);
					result = (string)jsonObject["CurrAccountHashCode"];
					file.Close();
				}
			}

			return result;
		}

		//读取监控信息
		private static JObject ReadMonitorInfo(
			string path,string account, int todayRebootCnt, int rebootMaxCnt)
		{
			JObject postedJObject = new JObject();
			if (!File.Exists(path))
			{
				postedJObject.Add("文件状态", "读取监控配置文件失败");
				return postedJObject;
			}
			using (StreamReader file = File.OpenText(path))
			{
				using (JsonTextReader reader = new JsonTextReader(file))
				{
					//读取
					JObject jsonObject = (JObject)JToken.ReadFrom(reader);
					int Wins = (int)jsonObject["Wins"];
					int Losses = (int)jsonObject["Losses"];
					int Concedes = (int)jsonObject["Concedes"];
					int Level = (int)jsonObject["Level"];
					int Xp = (int)jsonObject["Xp"];
					int XpNeeded = (int)jsonObject["XpNeeded"];
					int AllXp = (int)jsonObject["AllXp"];
					int AllXpNeeded = (int)jsonObject["AllXpNeeded"];
					string AllRunTimeText = (string)jsonObject["AllRunTimeText"];
					long AllGetXp = (long)jsonObject["AllGetXp"];
					int PerHourXp = (int)jsonObject["PerHourXp"];
					string PerHourXpStr = (string)jsonObject["PerHourXpStr"];
					int FullXpNeeded = (int)jsonObject["FullXpNeeded"];
					string FullTimeNeeded = (string)jsonObject["FullTimeNeeded"];
					string Collection = (string)jsonObject["Collection"];
					string PassportEnd = (string)jsonObject["PassportEnd"];
					string TwistInfo = (string)jsonObject["TwistInfo"];
					string StandardInfo = (string)jsonObject["StandardInfo"];
					string WildInfo = (string)jsonObject["WildInfo"];

					//组装
					postedJObject.Add("0", string.Format("战令等级={0}级 账号={1}", Level, account));
					postedJObject.Add("1", string.Format("升级经验={0}/{1} 重启次数={2}/{3}",
						Xp, XpNeeded, todayRebootCnt, rebootMaxCnt));
					postedJObject.Add("2", string.Format("总体经验={0}/{1} 时长={2}小时",
						AllXp, AllXpNeeded, AllRunTimeText));
					postedJObject.Add("3", string.Format("满级还差={0}经验 {1}",
						FullXpNeeded, FullTimeNeeded));
					postedJObject.Add("4", string.Format("获取经验={0} 挂机效率={1}/小时",
						AllGetXp, PerHourXp));
					postedJObject.Add("5", string.Format("账号资产={0}", Collection));
					postedJObject.Add("6", string.Format("战令结束={0}", PassportEnd));
					postedJObject.Add("7", string.Format("幻变天梯={0}", TwistInfo));
					postedJObject.Add("8", string.Format("标准天梯={0}", StandardInfo));
					postedJObject.Add("9", string.Format("狂野天梯={0}", WildInfo));

					file.Close();
				}
			}
			return postedJObject;
		}

		//组装成Json网络发送数据
		private static string StrToJsonSend(string token, string title, JObject content, string level = "active")
		{
			JObject postedJObject = new JObject();
			postedJObject.Add("title", title);
			if (token.StartsWith("http"))
			{
				var lines = content.Properties().Select(p => $"{p.Name}: {p.Value}");
				
				postedJObject.Add("body", string.Join("\n", lines));
				postedJObject.Add("group", "hearthbuddy");
				postedJObject.Add("level", level);
			}
			else
			{
				postedJObject.Add("token", token);
				postedJObject.Add("content", content);
				postedJObject.Add("template", "json");
			}

			return postedJObject.ToString();
		}

		//发送消息
		public enum MSG_TYPE{MSG_NORMAL=0, MSG_REBOOT, MSG_START, MSG_STOP, MSG_CHANGE, MSG_TEST};
		public static bool PostMessage(
			string token,
			string oldAccount,string account, 
			string rule,
			int todayRebootCnt, int rebootMaxCnt,
			MSG_TYPE type, out string result)
		{
			string sendData="";

			//参数判断
			if (token.Length<=0)
			{
				result = "请先获取Token后重试";
				return false;
			}

			//组装消息
			if (type == MSG_TYPE.MSG_TEST)
			{
				JObject postedJObject = new JObject();
				postedJObject.Add("当前状态", "微信推送功能设置成功");
				sendData = StrToJsonSend(token, "恭喜恭喜", postedJObject);
			}
			else if (type == MSG_TYPE.MSG_NORMAL)
			{
				string path_account =
					"Settings/Default/Monitor" + ReadCurrAccount("Settings/Default/Dev.json") + ".json";
				JObject postedJObject = ReadMonitorInfo(
					path_account, UtilsCom.ReplaceWithSpecialChar(account), todayRebootCnt, rebootMaxCnt);
				sendData = StrToJsonSend(token, "运行正常--账号(" + UtilsCom.ReplaceWithSpecialChar(account) + ")", postedJObject, "passive");
			}
			else if (type == MSG_TYPE.MSG_REBOOT)
			{
				string path_account =
					"Settings/Default/Monitor" + ReadCurrAccount("Settings/Default/Dev.json") + ".json";
				JObject postedJObject = ReadMonitorInfo(
					path_account, UtilsCom.ReplaceWithSpecialChar(account), todayRebootCnt, rebootMaxCnt);
				sendData = StrToJsonSend(token, "运行异常--账号(" + UtilsCom.ReplaceWithSpecialChar(account) + ")", postedJObject);
			}
			else if (type == MSG_TYPE.MSG_START)
			{
				JObject postedJObject = new JObject();
				postedJObject.Add("当前状态", "兄弟中控自动开始运行");
				postedJObject.Add("重启次数", todayRebootCnt.ToString() + "/" + rebootMaxCnt.ToString());
				sendData = StrToJsonSend(token, "兄弟中控已开始运行", postedJObject);
			}
			else if (type == MSG_TYPE.MSG_STOP)
			{
				JObject postedJObject = new JObject();
				postedJObject.Add("当前状态", "兄弟中控自动停止运行");
				postedJObject.Add("重启次数", todayRebootCnt.ToString() + "/" + rebootMaxCnt.ToString());
				sendData = StrToJsonSend(token, "兄弟中控已停止运行", postedJObject);
			}
			else if (type == MSG_TYPE.MSG_CHANGE)
			{
				string path_account =
					"Settings/Default/Monitor" + ReadCurrAccount("Settings/Default/Dev.json") + ".json";
				JObject postedJObject = ReadMonitorInfo(path_account,
					UtilsCom.ReplaceWithSpecialChar(oldAccount), todayRebootCnt, rebootMaxCnt);
				sendData = StrToJsonSend(token, "账号切换到(" + UtilsCom.ReplaceWithSpecialChar(account) + ")" +
					"[" + rule + "]", postedJObject);
			}

			try
			{
				//初始化
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.pushplus.plus/send");
				if (token.StartsWith("http"))
				{
					req = (HttpWebRequest)WebRequest.Create(token);
				}
				
				req.Method = "POST";
				req.ContentType = "application/json";
				byte[] data = Encoding.UTF8.GetBytes(sendData);
				req.ContentLength = data.Length;

				//发送请求
				using (Stream reqStream = req.GetRequestStream())
				{
					reqStream.Write(data, 0, data.Length);
					reqStream.Close();
				}
				HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
				Stream stream = resp.GetResponseStream();

				//获取响应内容
				using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
				{
					result = reader.ReadToEnd();
					if (result.Contains("200"))
					{
						result = "消息推送成功";
						return true;
					}
					else
					{
						result = "消息推送异常";
						return false;
					}
				}
			}
			catch(Exception ex)
			{
				ex.ToString();
				result = "网络异常";
				return false;
			}
		}
	}
}