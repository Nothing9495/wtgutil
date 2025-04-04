using Microsoft.Win32;
using System;
using System.Linq;
using System.Security.Principal;
using static WTG_Utility_DeviceInfo;

namespace WTG_Utility.Functions_CHS
{
    internal class IsAdmin
    {
        public static void IsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();    //Privilege Check
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                bool userGroup = principal.IsInRole(WindowsBuiltInRole.Administrator);
                if (userGroup == true)
                {
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("需要提升权限才能运行 wtgutil.");
                    Console.WriteLine();
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }

    internal class GetSettings
    {
        internal static void GetBootDriverFlags()
        {
            try
            {
                RegistryKey getBDF = Registry.LocalMachine.OpenSubKey("SYSTEM\\HardwareConfig\\Current");
                int statusBDF = (int)getBDF.GetValue("BootDriverFlags");
                if (statusBDF == 20)                                           //BootDriverFlags Check
                {
                    Console.WriteLine("  从 USB 设备启动：支持");
                }
                else if (statusBDF == 28)
                {
                    Console.WriteLine("  从 USB 设备启动：支持");
                }
                else if (statusBDF == 0)
                {
                    Console.WriteLine("  从 USB 设备启动：不支持");
                }
                else
                {
                    Console.WriteLine("  从 USB 设备启动：状态未知");
                }
                getBDF.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  从 USB 设备启动：发生错误: {ex.Message}");
            }

        }

        internal static void GetPortableOSFeature()
        {
            try
            {
                RegistryKey getPOS = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
                bool existPOS = (getPOS.GetValueNames().Contains("PortableOperatingSystem"));
                if (existPOS == true)
                {
                    int statusPOS = (int)getPOS.GetValue("PortableOperatingSystem");
                    if (statusPOS == 1)                                            //PortableOS Check
                    {
                        Console.WriteLine("  WinToGo 特性：   已启用");
                    }
                    else if (statusPOS == 0)
                    {
                        Console.WriteLine("  WinToGo 特性：   已禁用");
                    }
                    else
                    {
                        Console.WriteLine("  WinToGo 特性：   状态未知");
                    }
                    getPOS.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  WinToGo 特性：   发生错误：{ex.Message}");
            }
        }

        internal static void GetPartmgrSettings()
        {
            try
            {
                RegistryKey getPMGR = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\partmgr\\Parameters");
                int statusPMGR = (int)getPMGR.GetValue("SanPolicy");
                if (statusPMGR == 4)                                            //Partmgr Check
                {
                    Console.WriteLine("  隐藏本地磁盘：   是");
                }
                else
                {
                    Console.WriteLine("  隐藏本地磁盘：   否");
                }
                getPMGR.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  隐藏本地磁盘：   发生错误: {ex.Message}");
            }

        }

        internal static void GetUASPStatus(string deviceInstancePath)
        {
            try
            {
                RegistryKey getUASP = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Enum\\" + deviceInstancePath);
                RegistryKey getUASPDriver = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\UASPStor\\");
                if ((int)getUASP.GetValue("Capabilities") == 0x00000094
                    && (string)getUASP.GetValue("DeviceDesc") == "@usbstor.inf,%genericbulkonly.devicedesc%;USB Mass Storage Device"
                    && (string)getUASP.GetValue("Mfg") == "@usbstor.inf,%generic.mfg%;Compatible USB storage device"
                    && (string)getUASP.GetValue("Service") == "USBSTOR")
                {
                    if ((string)getUASPDriver.GetValue("ImagePath") == "\\SystemRoot\\System32\\drivers\\USBSTOR.SYS")
                    {
                        Console.WriteLine("  UASP 状态:       已禁用，且应用了强制禁用");
                    }
                    else
                    {
                        Console.WriteLine("  UASP 状态:       已禁用");
                    }
                }
                else if ((int)getUASP.GetValue("Capabilities") == 0x00000094
                    || (string)getUASP.GetValue("DeviceDesc") == "@usbstor.inf,%genericbulkonly.devicedesc%;USB Mass Storage Device"
                    || (string)getUASP.GetValue("Mfg") == "@usbstor.inf,%generic.mfg%;Compatible USB storage device"
                    || (string)getUASP.GetValue("Service") == "USBSTOR")
                {
                    Console.WriteLine("  UASP 状态:        未知");
                }
                else
                {
                    if ((string)getUASPDriver.GetValue("ImagePath") == "\\SystemRoot\\System32\\drivers\\USBSTOR.SYS")
                    {
                        Console.WriteLine("  UASP 状态:       已强制禁用");
                    }
                    else
                    {
                        Console.WriteLine("  UASP 状态:       已启用");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(NullReferenceException))
                {
                    Console.WriteLine($"  UASP 状态:       发生错误. 请确保已连接WinToGo驱动器.");
                }
            }

        }

        internal static string GetWTGDriveInstancePath()
        {
            try
            {
                return FindScsiStorageDevices();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  获取设备实例路径时发生错误: {ex.Message}");
                return string.Empty;
            }
        }
    }

    internal class ModifySettings
    {
        internal static void SetBootDriverFlags(int value)
        {
            try
            {
                RegistryKey setBDF = Registry.LocalMachine.CreateSubKey("SYSTEM\\HardwareConfig\\Current");
                setBDF.SetValue("BootDriverFlags", value, RegistryValueKind.DWord);
                setBDF.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  设置 BootDriverFlags 时发生错误: {ex.Message}");
            }
        }

        internal static void SetPortableOSFeature(int value)
        {
            try
            {
                RegistryKey setPOS = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Control");
                setPOS.SetValue("PortableOperatingSystem", value, RegistryValueKind.DWord);
                setPOS.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  设置WinToGo特性时发生错误: {ex.Message}");
            }
        }

        internal static void SetPartmgrSettings(int value)
        {
            try
            {
                RegistryKey setPMGR = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\partmgr\\Parameters");
                setPMGR.SetValue("SanPolicy", value, RegistryValueKind.DWord);
                setPMGR.Close(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  设置 PartmgrSettings 时发生错误: {ex.Message}");
            }
        }

        internal static void DisableUASP(string deviceInstancePath)
        {
            try
            {
                RegistryKey disableUASP = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Enum\\" + deviceInstancePath);
                disableUASP.SetValue("Capabilities", 0x00000094, RegistryValueKind.DWord);
                disableUASP.SetValue("DeviceDesc", "@usbstor.inf,%genericbulkonly.devicedesc%;USB Mass Storage Device", RegistryValueKind.String);
                disableUASP.SetValue("Mfg", "@usbstor.inf,%generic.mfg%;Compatible USB storage device", RegistryValueKind.String);
                disableUASP.SetValue("Service", "USBSTOR", RegistryValueKind.String);
                disableUASP.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  禁用 UASP 时发生错误: {ex.Message}");
            }

        }

        internal static void DisableUASPbyReplaceDriverConfig()
        {
            try
            {
                RegistryKey disableUASPbyReplaceDriverConfig = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\UASPStor\\");
                disableUASPbyReplaceDriverConfig.SetValue("ImagePath", "\\SystemRoot\\System32\\drivers\\USBSTOR.SYS", RegistryValueKind.String);
                disableUASPbyReplaceDriverConfig.SetValue("Owners", "usbstor.inf\r\nv_mscdsc.inf\r\n", RegistryValueKind.String);
                disableUASPbyReplaceDriverConfig.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  通过驱动配置禁用 UASP 时发生错误: {ex.Message}");
            }

        }

        internal static void RestoreUASPDriverConfig()
        {
            try
            {
                RegistryKey RestoreUASPDriverConfig = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\UASPStor\\");
                RestoreUASPDriverConfig.SetValue("ImagePath", "\\SystemRoot\\System32\\drivers\\uaspstor.sys", RegistryValueKind.String);
                RestoreUASPDriverConfig.SetValue("Owners", "uaspstor.inf\r\n", RegistryValueKind.String);
                RestoreUASPDriverConfig.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  恢复 UASP 驱动程序配置时发生错误: {ex.Message}");
            }
        }
    }
}
