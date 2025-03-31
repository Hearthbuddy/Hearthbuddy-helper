using System;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace HearthHelper
{
	public class UtilsPatch
	{
		//打炉石补丁
		public static bool PatchHearthStone(string path,int patchType)
		{
			try
			{
				if (path.Length <= 0) return false;
                string plugins = Path.Combine(path + "/Hearthstone_Data/Plugins/x86", "libacsdk_x86.dll");
                string plugins_bk = Path.Combine(path + "/Hearthstone_Data/Plugins/x86", "libacsdk_x86.dll.back");
                path =Path.Combine(path + "/Hearthstone_Data/Managed", "Assembly-CSharp.dll");

                //备份文件
                if (patchType == 0)
                {
                    File.Copy(path, path + ".bak", true);
                    File.Copy(plugins, plugins_bk, true);
                }
                //还原文件
                else if (patchType == 6)
                {
                    if (File.Exists(path + ".bak"))
                    {
                        File.Copy(path + ".bak", path, true);
                        File.Delete(path + ".bak");
                    }
                    if (File.Exists(plugins_bk))
                    {
                        File.Copy(plugins_bk, plugins, true);
                        File.Delete(plugins_bk);
                    }
                }
                //窗口最小化
                else if (patchType == 1)
				{
                    string temp = path;
                    File.Copy(path, path + ".tmp", true);
                    var resolver = new DefaultAssemblyResolver();
                    resolver.AddSearchDirectory(temp.Replace("Assembly-CSharp.dll", ""));
                    var assembly = AssemblyDefinition.ReadAssembly(path + ".tmp",
                        new ReaderParameters { AssemblyResolver = resolver });
                    foreach (TypeDefinition type in assembly.MainModule.Types)
					{
						if (type.Name == "GraphicsResolution")
						{
							foreach (MethodDefinition method in type.Methods)
							{
								if (method.Name == "IsAspectRatioWithinLimit")
								{
									foreach (Instruction ins in method.Body.Instructions)
									{
										if (ins.OpCode.Name == "brfalse.s")
										{
											ins.OpCode = OpCodes.Brtrue_S;
										}
									}
									break;
								}
							}
							break;
						}
					}
                    //保存文件
                    assembly.Write(path);
                    assembly.Dispose();
                    File.Delete(path + ".tmp");
                }
                //反作弊
                else if (patchType == 2)
                {
                    string temp = path;
                    File.Copy(path, path + ".tmp", true);
                    var resolver = new DefaultAssemblyResolver();
                    resolver.AddSearchDirectory(temp.Replace("Assembly-CSharp.dll", ""));
                    var assembly = AssemblyDefinition.ReadAssembly(path + ".tmp",
                        new ReaderParameters { AssemblyResolver = resolver });
                    var targetType = assembly.MainModule.GetType("AntiCheatSDK.AntiCheatManager");
                    foreach (var targetMethod in targetType.Methods)
                    {
                        if (targetMethod.Name == "CallInterfaceSetupSDK" ||
                            targetMethod.Name == "ClearExtraParams" ||
                            targetMethod.Name == "InnerSDKMethodCall" ||
                            targetMethod.Name == "OnLoginComplete" ||
                            targetMethod.Name == "WriteUserInfo")
                        {
                            MethodBody body;
                            try { body = targetMethod.Body; } catch { continue; }

                            // 清空方法体
                            body.Instructions.Clear();
                            var processor = body.GetILProcessor();

                            // 添加返回语句（因为该方法没有返回值）
                            processor.Append(processor.Create(OpCodes.Ret));
                        }
                    }
                    //保存文件
                    assembly.Write(path);
                    assembly.Dispose();
                    File.Delete(path + ".tmp");
                    File.Delete(plugins);
                }
                //去除广告
                else if (patchType == 3)
				{
                    string temp = path;
                    File.Copy(path, path + ".tmp", true);
                    var resolver = new DefaultAssemblyResolver();
                    resolver.AddSearchDirectory(temp.Replace("Assembly-CSharp.dll", ""));
                    var assembly = AssemblyDefinition.ReadAssembly(path + ".tmp",
                        new ReaderParameters { AssemblyResolver = resolver });
                    foreach (TypeDefinition type in assembly.MainModule.Types)
					{
						if (type.Name == "ViewCountController")
						{
							foreach (MethodDefinition method in type.Methods)
							{
								if (method.Name == "GetViewCount")
								{
									method.Body.Instructions[9].OpCode = OpCodes.Ldc_I4_1;
									break;
								}
							}
							break;
						}
					}
                    //保存文件
                    assembly.Write(path);
                    assembly.Dispose();
                    File.Delete(path + ".tmp");
                }
				//去除特有开门
				else if (patchType == 4)
				{
                    string temp = path;
                    File.Copy(path, path + ".tmp", true);
                    var resolver = new DefaultAssemblyResolver();
                    resolver.AddSearchDirectory(temp.Replace("Assembly-CSharp.dll", ""));
                    var assembly = AssemblyDefinition.ReadAssembly(path + ".tmp",
                        new ReaderParameters { AssemblyResolver = resolver });
                    foreach (TypeDefinition type in assembly.MainModule.Types)
					{
						if (type.Name == "SplashScreen")
						{
							foreach (MethodDefinition method in type.Methods)
							{
								if (method.Name == "GetRatingsScreenRegion")
								{
									method.Body.Instructions[0].OpCode = OpCodes.Ret;
									break;
								}
							}
							break;
						}
					}
                    //保存文件
                    assembly.Write(path);
                    assembly.Dispose();
                    File.Delete(path + ".tmp");
                }
				//去除金卡特效
				else if (patchType == 5)
				{
                    string temp = path;
                    File.Copy(path, path + ".tmp", true);
                    var resolver = new DefaultAssemblyResolver();
                    resolver.AddSearchDirectory(temp.Replace("Assembly-CSharp.dll", ""));
                    var assembly = AssemblyDefinition.ReadAssembly(path + ".tmp",
                        new ReaderParameters { AssemblyResolver = resolver });
                    foreach (TypeDefinition type in assembly.MainModule.Types)
					{
						if (type.Name == "Entity")
						{
							foreach (MethodDefinition method in type.Methods)
							{
								if (method.Name == "GetPremiumType")
								{
									method.Body.Instructions[11].OpCode = OpCodes.Ldc_I4_0;
									method.Body.Instructions[13].OpCode = OpCodes.Ldc_I4_0;
									break;
								}
							}
							break;
						}
					}
                    //保存文件
                    assembly.Write(path);
                    assembly.Dispose();
                    File.Delete(path + ".tmp");
                }

				return true;
			}
			catch (Exception e)
			{
                System.Windows.MessageBox.Show(
					e.ToString(),"",MessageBoxButton.OK);
				return false;
			}
		}
	}
}