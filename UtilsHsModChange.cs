using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HearthHelper
{
    public class UtilsHsModChange
    {
        private static HttpClient Http = new HttpClient();

        private static readonly Regex ConfigRegex = new Regex(@"^\s*(?<key>[^=\s]+)\s*=\s*(?<value>.*)\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex MetadataRegex =
            new Regex(@"^##?\s*(.*)$", RegexOptions.Compiled | RegexOptions.Multiline);

        public static string url = "localhost";
        public static Dictionary<string, CfgData> hsmodcfg = new Dictionary<string, CfgData>();

        public static Dictionary<string, Dictionary<string, string>> skins =
            new Dictionary<string, Dictionary<string, string>>();

        public static void CheckHsModChange(int port, bool EnableGMMessageShow, bool EnableEnemyEmote,
            bool EnableQuickMode,
            bool EnableRankInGameShow, bool EnableCardState)
        {
            if (hsmodcfg.Count == 0)
            {
                Task.Run(() => GetHsmodCfg(port)).GetAwaiter().GetResult();
                if (hsmodcfg.Count == 0)
                {
                    return;
                }
            }

            // 广告推销、削弱补丁、天梯结算信息等
            hsmodcfg.TryGetValue("游戏内消息", out CfgData EnableGMMessageShowCfgData);
            if (EnableGMMessageShow == EnableGMMessageShowCfgData.boolValue)
            {
                EnableGMMessageShowCfgData.boolValue = !EnableGMMessageShow;
            }

            // 屏蔽对手表情
            hsmodcfg.TryGetValue("表情数量", out CfgData EnableEnemyEmoteCfgData);
            if (EnableEnemyEmote && EnableEnemyEmoteCfgData.intValue != 0)
            {
                EnableEnemyEmoteCfgData.intValue = 0;
            }
            else if (!EnableEnemyEmote && EnableEnemyEmoteCfgData.intValue == 0)
            {
                EnableEnemyEmoteCfgData.intValue = -1;
            }

            // 快速战斗
            hsmodcfg.TryGetValue("快速战斗", out CfgData EnableQuickModeCfgData);
            if (EnableQuickMode != EnableQuickModeCfgData.boolValue)
            {
                EnableQuickModeCfgData.boolValue = EnableQuickMode;
            }

            // 显示天梯等级
            hsmodcfg.TryGetValue("显示天梯等级", out CfgData EnableRankInGameShowCfgData);
            if (EnableRankInGameShow != EnableRankInGameShowCfgData.boolValue)
            {
                EnableRankInGameShowCfgData.boolValue = EnableRankInGameShow;
            }

            // 卡牌特效
            hsmodcfg.TryGetValue("卡牌最高特效", out CfgData EnableCardStateCfgData);
            if (EnableCardState && !EnableCardStateCfgData.stringValue.Equals("Disabled"))
            {
                EnableCardStateCfgData.stringValue = "Disabled";
            }
            else if (!EnableCardState && !EnableCardStateCfgData.stringValue.Equals("Default"))
            {
                EnableCardStateCfgData.stringValue = "Default";
            }

            Task.Run(() => SyncHsModChange(port)).GetAwaiter().GetResult();
        }

        public static async Task SyncHsModChange(int port)
        {
            try
            {
                if (hsmodcfg == null)
                {
                    return;
                }

                foreach (CfgData val in hsmodcfg.Values)
                {
                    if (val.changed)
                    {
                        var requestData = new RequestData
                        {
                            key = val.key,
                            value = val.value
                        };
                        
                        string json = JsonConvert.SerializeObject(requestData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await Http.PostAsync($"http://{url}:{port}/config", content);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        UtilsCom.Log($"修改HsMod配置成功{val.key}:{val.value}");
                        val.changed = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }

        public static async Task GetHsmodCfg(int port)
        {
            try
            {
                // 发送GET请求
                HttpResponseMessage response = await Http.GetAsync($"http://{url}:{port}/hsmod.cfg");
                response.EnsureSuccessStatusCode();

                string cfgResponse = await response.Content.ReadAsStringAsync();

                string[] blocks = cfgResponse.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None);
                var data = new Dictionary<string, CfgData>();
                foreach (string block in blocks)
                {
                    var matches = ConfigRegex.Matches(block);
                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            string key = match.Groups["key"].Value.Trim();
                            string value = match.Groups["value"].Value.Trim();
                            var metadataMatches = MetadataRegex.Matches(block);
                            string type = "";
                            string defaultValue = "";
                            string ps = "";
                            string acceptValue = "";
                            string acceptableRange = "";
                            foreach (Match metadataMatch in metadataMatches)
                            {
                                string comment = metadataMatch.Groups[1].Value.Trim();

                                if (comment.StartsWith("Setting type:"))
                                {
                                    type = comment.Substring("Setting type:".Length).Trim();
                                }
                                else if (comment.StartsWith("Default value:"))
                                {
                                    defaultValue = comment.Substring("Default value:".Length).Trim();
                                }
                                else if (comment.StartsWith("Acceptable values:"))
                                {
                                    acceptValue = comment.Substring("Acceptable values:".Length).Trim();
                                }
                                else if (comment.StartsWith("Acceptable value range:"))
                                {
                                    acceptableRange = comment.Substring("Acceptable value range:".Length).Trim();
                                }
                                else
                                {
                                    ps = comment;
                                }
                            }

                            data[key] = new CfgData(key, type, value, defaultValue, ps, acceptValue, acceptableRange);
                        }
                    }
                }

                hsmodcfg = data;
            }

            catch (Exception e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }

        private Dictionary<string, string> ParseConfig(string cfg)
        {
            var config = new Dictionary<string, string>();
            var matches = ConfigRegex.Matches(cfg);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string key = match.Groups["key"].Value.Trim();
                    string value = match.Groups["value"].Value.Trim();
                    config[key] = value;
                }
            }

            return config;
        }

        public class RequestData
        {
            public string key { get; set; }
            public string value { get; set; }
        }
    }

    public class CfgData
    {
        public string key { get; set; }
        public Type type { get; set; }
        public string value;

        public string stringValue
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.changed = true;
            }
        }

        public int intValue
        {
            get { return int.Parse(this.value); }
            set
            {
                this.value = value.ToString();
                this.changed = true;
            }
        }

        public bool boolValue
        {
            get { return Convert.ToBoolean(this.value); }
            set
            {
                this.value = value.ToString();
                this.changed = true;
            }
        }

        public string tranValue
        {
            get { return UtilsHsModChange.skins[this.key][this.value]; }
            set
            {
                this.value = UtilsHsModChange.skins[this.key].FirstOrDefault(q => q.Value == value.ToString()).Key;
                this.changed = true;
            }
        }

        public bool changed = false;
        public string defaultValue { get; set; }
        public string ps { get; set; }
        public List<string> acceptValue { get; set; }
        public List<int> acceptableRange { get; set; }

        public CfgData(string key, string type, string value, string defaultValue, string ps, string acceptValue,
            string acceptableRange)
        {
            this.key = key;
            if (type == "Boolean")
            {
                this.type = typeof(bool);
                this.acceptValue = null;
                this.acceptableRange = null;
            }
            else if (type == "Int32")
            {
                this.type = typeof(int);
                this.acceptValue = null;
                this.acceptableRange = Regex.Matches(acceptableRange, @"-?\d+")
                    .Cast<Match>()
                    .Select(match => int.Parse(match.Value))
                    .ToList();
            }
            else
            {
                this.type = typeof(string);
                this.acceptValue = acceptValue.Split(',')
                    .Select(word => word.Trim()) // 去除多余的空格
                    .ToList();
                this.acceptableRange = null;
            }

            this.value = value;
            this.defaultValue = defaultValue;
            this.ps = ps;
            this.stringValue = value;
            this.changed = false;
        }

        public T getValue<T>()
        {
            Type t = typeof(T);
            if (t == typeof(int))
            {
                return (T)(object)int.Parse(value);
            }
            else if (t == typeof(string))
            {
                return (T)(object)value;
            }
            else if (t == typeof(bool))
            {
                return (T)(object)Convert.ToBoolean(value);
            }

            return default(T);
        }
    }
}