using System;
using System.Management;
using System.Security.Cryptography;
using System.Security;
using System.Collections;
using System.Text;
using System.Windows.Input;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace HearthHelper
{
    public static class Rsa
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="str">需签名的数据</param>
        /// <returns>签名后的值</returns>
        public static string RsaSign(string str)
        {
            byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(str);
            var sha256 = new SHA256CryptoServiceProvider();
            byte[] rgbHash = sha256.ComputeHash(bt);
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(File.ReadAllText("privateKey.xml"));
            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
            formatter.SetHashAlgorithm("SHA256");
            byte[] inArray = formatter.CreateSignature(rgbHash);
            return Convert.ToBase64String(inArray);
        }

        public static void AddUser()
        {
            //用户
            File.Delete("Users.dat");
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(xmlDeclaration);
            XmlElement rootElement = doc.CreateElement("", "Users", "");
            doc.AppendChild(rootElement);
            string filePath = "Users.txt";
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    XmlElement childElement = doc.CreateElement("", "Key", "");
                    childElement.InnerText = RsaSign(line);
                    rootElement.AppendChild(childElement);
                }
            }
            File.WriteAllText("Users.dat", AesEncrypt(doc.OuterXml));

            //能力集
            File.Delete("Ability.dat");
            XDocument doc2 = XDocument.Load("Ability.xml");
            File.WriteAllText("Ability.dat", AesEncrypt(doc2.ToString()));
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="str">待验证的字符串</param>
        /// <param name="sign">加签之后的字符串</param>
        /// <returns>签名是否符合</returns>
        public static bool RsaSignCheck(string str, string sign)
        {
            try
            {
                string xmlPublicKey = "<RSAKeyValue><Modulus>l9YgeWOlHPRczcPwccVAJQcyB6lFcNIPiQUR41ZpEV2AUePTboMisPOqQFfik91Vbi5ooFTEoFIi2yUQiIOcAEGyka4o0m4JsJg+ZWruBdTqZY+7fckKQb61hM9Cf1f88yM/4DB+u8iQFtg1fYZ45o5ldN62Kd9ku4pTxbLB4iOptXQxyzDyYrA59wKDFC4XMW8LKFqXtv3oW39UWdLU/nXYqezDlhki9W3RRYctF3knWNF4xRTAqQEz6AzUJR4icrp2WGl0t3cHFiK8kEMmCenRr7LfkuMoUpiudOVLzrPdYhkroaZ/Zje+uDRB7QgFlaIYhAfKrzqLfTasiczdQQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
                byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(str);
                var sha256 = new SHA256CryptoServiceProvider();
                byte[] rgbHash = sha256.ComputeHash(bt);
                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(xmlPublicKey);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
                deformatter.SetHashAlgorithm("SHA256");
                byte[] rgbSignature = Convert.FromBase64String(sign);
                if (deformatter.VerifySignature(rgbHash, rgbSignature))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string AesEncrypt(string data)
        {
            try
            {
                string base64Key = "nM2SaGfvwlqiJ4I7D3mFQ7AFtDnzNCM/ptOi299ISaU=";
                string base64Iv = "yp2+XGZ9J4K4dnyfgF4kGg==";
                byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(data);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Convert.FromBase64String(base64Key);
                    aes.IV = Convert.FromBase64String(base64Iv);
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt,
                            encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            csEncrypt.FlushFinalBlock();
                            byte[] encryptedBytes = msEncrypt.ToArray();
                            return Convert.ToBase64String(encryptedBytes);
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        public static string AesDecrypt(string base64EncryptedData)
        {
            try
            {
                string base64Key = "nM2SaGfvwlqiJ4I7D3mFQ7AFtDnzNCM/ptOi299ISaU=";
                string base64Iv = "yp2+XGZ9J4K4dnyfgF4kGg==";
                byte[] encryptedBytes = Convert.FromBase64String(base64EncryptedData);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Convert.FromBase64String(base64Key);
                    aes.IV = Convert.FromBase64String(base64Iv);
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (var memoryStream = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                            decryptor, CryptoStreamMode.Read))
                        {
                            using (var streamReader = new StreamReader(cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
