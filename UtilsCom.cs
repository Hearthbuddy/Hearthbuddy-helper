using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace HearthHelper
{
    public class UtilsCom
    {
        //日志
        public static Queue logQueue = new Queue();
        public static void Log(string log)
        {
            logQueue.Enqueue(log);
        }

        //延时函数
        public static void Delay(int mm)
        {
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(mm) > DateTime.Now)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }

        //隐藏邮箱
        public static string ReplaceWithSpecialChar(
            string src, char specialChar = '*')
        {
            if (src.Length <= 0) return "";
            string tmp = src;
            try
            {
                int startLen = 0, speciLen = 0, endLen = 0;
                string replaceStr, specialStr;
                if (tmp.Contains("@")) //邮箱账号
                {
                    string[] sArray = Regex.Split(tmp, "@", RegexOptions.IgnoreCase);
                    int accountLen = sArray[0].Length;
                    startLen = 1;
                    endLen = 1;
                    if (accountLen < 2) endLen = 0;
                    if (accountLen >= 7)
                    {
                        startLen = 2;
                        endLen = 2;
                    }
                    speciLen = accountLen - startLen - endLen;
                }
                else//手机账号
                {
                    startLen = 1;
                    endLen = 1;
                    if (tmp.Length < 2) endLen = 0;
                    if (tmp.Length >= 7)
                    {
                        startLen = 3;
                        endLen = 2;
                    }
                    speciLen = tmp.Length - startLen - endLen;
                }
                if (speciLen > 0)
                {
                    replaceStr = tmp.Substring(startLen, speciLen);
                    specialStr = string.Empty;
                    for (int i = 0; i < replaceStr.Length; i++)
                    {
                        specialStr += specialChar;
                    }
                    tmp = tmp.Replace(replaceStr, specialStr);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tmp;
        }

        /// 检测是否有中文字符
        public static bool IsHasCHZN(string inputData)
        {
            Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
            Match m = RegCHZN.Match(inputData);
            return m.Success;
        }
    }
}

