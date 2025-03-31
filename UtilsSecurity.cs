using System;
using System.Management;
using System.Security.Cryptography;
using System.Security;
using System.Collections;
using System.Text;
using System.Xml;
using System.Net;
using System.Windows.Documents;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;
using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;

namespace HearthHelper
{
    public static class Security
    {
        public static int bAuthOK()
        {
            try
            {
                string dataString = "";
                string xmlString = "";
                WebClient wc = new WebClient();
                using (StreamReader reader = new StreamReader(wc.OpenRead("https://gitee.com/UniverseString/Hearthstone-myAccount/raw/master/myAccount/Users.dat")))
                {
                    dataString = reader.ReadToEnd();
                }
                xmlString =Rsa.AesDecrypt(dataString);
                byte[] byteArray = Encoding.UTF8.GetBytes(xmlString);
                Stream stream = new MemoryStream(byteArray);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(stream);
                XmlNode list = xmlDoc.SelectSingleNode("Users");
                wc.Dispose();
                bool bUserAuthOk = false;
                string key = Token();
                if (key == "c82ad3d391d1b75cf2b85aef8a1a5ce0")
                {
                    key = Token1();
                }
                if (key != null)
                {
                    foreach (XmlNode node in list)
                    {
                        if (node.Name == "Key")
                        {
                            if (Rsa.RsaSignCheck(key, node.InnerText))
                            {
                                bUserAuthOk = true;
                                break;
                            }
                        }
                    }
                    if (bUserAuthOk)
                    {
                        UtilsCom.Log("当前用户认证成功，正常使用！");
                        return 0;
                    }
                    if (!bUserAuthOk)
                    {
                        UtilsCom.Log("当前用户认证失败，无法使用！");
                        UtilsCom.Log("请将下面Key复制后发送给管理员进行注册！");
                        UtilsCom.Log($"Key={key}");
                    }
                }
                return 1;
            }
            catch (Exception)
            {
                string key = Token();
                if (key == "c82ad3d391d1b75cf2b85aef8a1a5ce0")
                {
                    key = Token1();
                }
                if (key != null)
                {
                    UtilsCom.Log("当前用户认证失败，无法使用！");
                    UtilsCom.Log("请将下面Key复制后发送给管理员进行注册！");
                    UtilsCom.Log($"Key={key}");
                }
                return -1;
            }
        }

        /// <summary>
        /// 获取设备硬件码
        /// </summary>
        /// <returns></returns>
        public static string Token()
        {
            string hd = GetHD();
            string mac = GetMoAddress();
            string cpu = GetCpuID();
            if (hd != null || mac != null || cpu != null)
            {
                string temp = hd + mac + cpu;
                return GetMD5String(temp);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取设备硬件码
        /// </summary>
        /// <returns></returns>
        public static string Token1()
        {
            string hd = GetHD();
            string mac = GetMoAddress1();
            string cpu = GetCpuID();
            if (hd != null || mac != null || cpu != null)
            {
                string temp = hd + mac + cpu;
                return GetMD5String(temp);
            }
            else
            {
                return "";
            }
        }

        public static string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码
                string cpuInfo = "";//cpu序列号
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
        }

        /// <summary>
        /// 获取硬盘ID  
        /// </summary>
        /// <returns></returns>
        public static string GetHD()
        {
            try
            {
                //查询得到系统盘所在硬盘的ID = 0。然后我们通过该ID，查询该硬盘信息。
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT DiskIndex FROM Win32_DiskPartition WHERE Bootable = TRUE");
                foreach (ManagementObject mooo in searcher.Get())
                {
                    int index = Convert.ToInt32(mooo.Properties["DiskIndex"].Value);
                    ManagementObjectSearcher searcher_model = new ManagementObjectSearcher("SELECT Model FROM Win32_DiskDrive WHERE Index = " + index);
                    ManagementObjectCollection moc1 = searcher_model.Get();
                    foreach (ManagementObject mo in moc1)
                    {
                        string modelname = (string)mo.Properties["Model"].Value;
                        return modelname;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///   <summary> 
        ///   获取网卡硬件地址 
        ///   </summary> 
        ///   <returns> string </returns> 
        public static string GetMoAddress()
        {
            try
            {
                string MoAddress = null;
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                {
                    ManagementObjectCollection moc2 = mc.GetInstances();
                    foreach (ManagementObject mo in moc2)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                            MoAddress = mo["MacAddress"].ToString();
                        mo.Dispose();
                    }
                }
                //return MoAddress;
                return "MoAddress";
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///   <summary> 
        ///   获取网卡硬件地址 
        ///   </summary> 
        ///   <returns> string </returns> 
        public static string GetMoAddress1()
        {
            try
            {
                string MoAddress = null;
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                {
                    ManagementObjectCollection moc2 = mc.GetInstances();
                    foreach (ManagementObject mo in moc2)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                            MoAddress = mo["MacAddress"].ToString();
                        mo.Dispose();
                    }
                }
                return MoAddress;
                //return "MoAddress";
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetSystemTime()
        {
            try
            {
                System.Management.ObjectQuery MyQuery = new System.Management.ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                System.Management.ManagementScope MyScope = new System.Management.ManagementScope();
                ManagementObjectSearcher MySearch = new ManagementObjectSearcher(MyScope, MyQuery);
                ManagementObjectCollection MyCollection = MySearch.Get();
                string StrInfo = "";
                foreach (ManagementObject MyObject in MyCollection)
                {
                    StrInfo = MyObject.GetText(TextFormat.Mof);
                }
                string InstallDate = StrInfo.Substring(StrInfo.LastIndexOf("InstallDate") + 15, 14);
                UtilsCom.Log($"SystemTime={InstallDate}");
                return null;
            }
            catch (Exception)
            {
                UtilsCom.Log($"SystemTime=111");
                return null;
            }
        }

        /// <summary>
        /// 通过字符串获取MD5值，返回32位字符串。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5String(string text)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(text));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString().ToLower();
        }
    }
}
