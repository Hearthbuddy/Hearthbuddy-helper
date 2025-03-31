using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace HearthHelper
{
    public static class UtilsCopy
    {
        //判断文件是否一致
        public static bool FileIsSame(string str1, string str2)
        {
            try 
            {
                string p_1 = str1;
                string p_2 = str2;

                //计算第一个文件的哈希值
                var hash = System.Security.Cryptography.HashAlgorithm.Create();
                var stream_1 = new System.IO.FileStream(p_1, System.IO.FileMode.Open);
                byte[] hashByte_1 = hash.ComputeHash(stream_1);
                stream_1.Close();

                //计算第二个文件的哈希值
                var stream_2 = new System.IO.FileStream(p_2, System.IO.FileMode.Open);
                byte[] hashByte_2 = hash.ComputeHash(stream_2);
                stream_2.Close();

                //比较两个哈希值
                return (BitConverter.ToString(hashByte_1) == BitConverter.ToString(hashByte_2));
            }
            catch
            {
                return true; 
            }
        }

        //复制文件夹下的所有文件、目录到目标文件夹
        public static void CopyFileAndDir(string srcDir, string desDir)
        {
            //创建文件夹
            if (!System.IO.Directory.Exists(desDir))
            {
                System.IO.Directory.CreateDirectory(desDir);
            }

            //遍历
            IEnumerable<string> files = System.IO.Directory.EnumerateFileSystemEntries(srcDir);
            if (files != null && files.Count() > 0)
            {
                foreach (var item in files)
                {
                    string desPath = System.IO.Path.Combine(desDir, System.IO.Path.GetFileName(item));

                     //如果是文件
                    if (System.IO.File.Exists(item))
                    {
                        if (!System.IO.File.Exists(desPath) || !FileIsSame(item,desPath))
                        {
                            //复制文件到目标文件夹
                            System.IO.File.Copy(item, desPath, true);
                        }
                        continue;
                    }

                    //如果是文件夹                   
                    CopyFileAndDir(item, desPath);
                }
            }
        }

        //复制佣兵插件文件到炉石目录
        public static void CopyMercFileToHearthPath(object HearthstonePath)
        {
            try
            {
                CopyFileAndDir(Path.Combine(Directory.GetCurrentDirectory(), "HsMod"),
                    (string)HearthstonePath);
            }
            catch { }
        }

        //删除文件夹
        public static void DeleteDirectory(string dir)
        {
            if (Directory.GetDirectories(dir).Length == 0 &&
                Directory.GetFiles(dir).Length == 0)
            {
                Directory.Delete(dir);
                return;
            }
            foreach (string var in Directory.GetDirectories(dir))
            {
                DeleteDirectory(var);
            }
            foreach (string var in Directory.GetFiles(dir))
            {
                File.Delete(var);
            }
            Directory.Delete(dir);
        }
    }
}
