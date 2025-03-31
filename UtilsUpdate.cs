using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Net;
using System.Windows.Documents;
using System.ComponentModel;
using System.Xml.Linq;
using Mono.Cecil.Cil;
using System.Text;

namespace HearthHelper
{
    public static class SoftUpdate
    {
        public static bool bDownloadOk=false;
        public static string filename = "";

        //下载线程
        public static void DownloadFile(object downloadurl_loadname)
        {
            try
            {
                string downloadurl = ((string)downloadurl_loadname).
                    Substring(0, ((string)downloadurl_loadname).IndexOf("@"));
                string downloadname = ((string)downloadurl_loadname).
                    Substring(((string)downloadurl_loadname).LastIndexOf("@") + 1);
                filename = downloadname.Substring(downloadname.LastIndexOf("HearthStudy"));
                WebClient wc = new WebClient();
                wc.DownloadFileCompleted += client_DownloadFileCompleted;
                wc.DownloadProgressChanged += client_DownloadProgressChanged;
                wc.DownloadFileAsync(new Uri(downloadurl),downloadname);
            }
            catch
            {
                UtilsCom.Log("升级包下载失败，网络连接异常，请稍后再试！");
            }
        }

        //下载完成
        public static bool bDownloadFileOk()
        {
            return bDownloadOk;
        }

        //下载完成
        static void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UtilsCom.Log("升级包下载完成，准备安装...");
            bDownloadOk = true;
        }

        //下载过程中
        static int progressPercentage = 0;
        static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (progressPercentage != e.ProgressPercentage)
            {
                progressPercentage = e.ProgressPercentage;
                if(progressPercentage>0)
                {
                    UtilsCom.Log($"升级包[{filename}]下载进度={progressPercentage}%");
                }
            }
        }

        //检查是否需要更新
        public static int bNeedUpdate(
            string oldVersion,ref string newVersion,
            ref string date,
            ref string downloadurl,ref string downloadname,
            ref string describe,
            ref string must, ref string auth,
            ref string enable,ref string reason)
        {
            try
            {
                string dataString = "";
                string xmlString = "";
                WebClient wc = new WebClient();
                using (StreamReader reader = new StreamReader(wc.OpenRead("https://gitee.com/UniverseString/Hearthstone-myAccount/raw/master/myAccount/Ability.dat")))
                {
                    dataString = reader.ReadToEnd();
                }
                xmlString = Rsa.AesDecrypt(dataString);
                byte[] byteArray = Encoding.UTF8.GetBytes(xmlString);
                Stream stream = new MemoryStream(byteArray);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(stream);
                XmlNode list = xmlDoc.SelectSingleNode("Ability");
                foreach (XmlNode node in list)
                {
                    if (node.Name == "Version")
                        newVersion = node.InnerText;
                    else if (node.Name == "Date")
                        date = node.InnerText;
                    else if (node.Name == "DownLoad")
                        downloadurl = node.InnerText;
                    else if (node.Name == "Describe")
                        describe = node.InnerText;
                    else if (node.Name == "Must")
                        must = node.InnerText;
                    else if (node.Name == "Auth")
                        auth = node.InnerText;
                    else if (node.Name == "Enable")
                        enable = node.InnerText;
                    else if (node.Name == "Reason")
                        reason = node.InnerText;
                }
                string rootPath=Directory.GetCurrentDirectory()+"\\";
                string filename = downloadurl.Substring(downloadurl.LastIndexOf("/")+1);
                downloadname = Path.Combine(
                    Path.GetDirectoryName(rootPath),"Update_"+filename);
                Version newver = new Version(newVersion);
                Version oldver = new Version(oldVersion);
                wc.Dispose();
                if (oldver.CompareTo(newver) >= 0) return 1;
                else return 0;
            }
            catch (Exception)
            {
                describe="检查更新失败，网络连接异常，请稍后再试！";
                return -1;
            }
        }
    }
}
