using Microsoft.Win32;
using System;

namespace WTG_Utility.Functions_CHS
{

    internal class GetSettings
    {
        internal static void CurrentInfo()
        {
            Console.WriteLine("当前系统信息:");
        }

        internal static void GetBootDriverFlags()
        {
            RegistryKey getBDF = Registry.LocalMachine.OpenSubKey("SYSTEM\\HardwareConfig\\Current");
            int statusBDF = (int)getBDF.GetValue("BootDriverFlags");
            getBDF.Close();

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
        }

        internal static void GetPortableOSFeature()
        {
            RegistryKey getPOS = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
            int statusPOS = (int)getPOS.GetValue("PortableOperatingSystem");
            getPOS.Close();

            if (statusPOS == 1)                                            //PortableOS Check
            {
                Console.WriteLine("  WinToGo 特性：已启用");
            }
            else if (statusPOS == 0)
            {
                Console.WriteLine("  WinToGo 特性：已禁用");
            }
            else
            {
                Console.WriteLine("  WinToGo 特性：状态未知");
            }
        }

        internal static void GetPartmgrSettings()
        {
            RegistryKey getPMGR = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\partmgr\\Parameters");
            int statusPMGR = (int)getPMGR.GetValue("SanPolicy");
            getPMGR.Close();

            if (statusPMGR == 4)                                            //Partmgr Check
            {
                Console.WriteLine("  隐藏本地磁盘：是");
            }
            else
            {
                Console.WriteLine("  隐藏本地磁盘：否");
            }
        }
    }

    internal class ModifySettings
    {
        internal static void SetBootDriverFlags(int value)
        {
            RegistryKey setBDF = Registry.LocalMachine.CreateSubKey("SYSTEM\\HardwareConfig\\Current");
            setBDF.SetValue("BootDriverFlags", value, RegistryValueKind.DWord);
            setBDF.Close();
        }

        internal static void SetPortableOSFeature(int value)
        {
            RegistryKey setPOS = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Control");
            setPOS.SetValue("PortableOperatingSystem", value, RegistryValueKind.DWord);
            setPOS.Close();
        }

        internal static void SetPartmgrSettings(int value)
        {
            RegistryKey setPMGR = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\partmgr\\Parameters");
            setPMGR.SetValue("SanPolicy", value, RegistryValueKind.DWord);
            setPMGR.Close();
        }
    }
}
