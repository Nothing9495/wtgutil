using Microsoft.Win32;
using System;
using System.Linq;
using System.Security.Principal;

namespace WTG_Utility.Functions
{
    internal class IsAdmin
    {
        public static void IsAdministrator()
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
                Console.WriteLine("Need to elevate privileges to run wtgutil.");
                Console.WriteLine();
                Environment.Exit(1);
            }
        }
    }

    internal class GetSettings
    {
        internal static void GetBootDriverFlags()
        {
            RegistryKey getBDF = Registry.LocalMachine.OpenSubKey("SYSTEM\\HardwareConfig\\Current");
            int statusBDF = (int)getBDF.GetValue("BootDriverFlags");
            if (statusBDF == 20)                                           //BootDriverFlags Check
            {
                Console.WriteLine("  Boot from USB Devices: Supported");
            }
            else if (statusBDF == 28)
            {
                Console.WriteLine("  Boot from USB Devices: Supported");
            }
            else if (statusBDF == 0)
            {
                Console.WriteLine("  Boot from USB Devices: Unsupported");
            }
            else
            {
                Console.WriteLine("  Boot from USB Devices: Status unknown");
            }
            getBDF.Close();
        }

        internal static void GetPortableOSFeature()
        {
            RegistryKey getPOS = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
            bool existPOS = (getPOS.GetValueNames().Contains("PortableOperatingSystem"));
            if (existPOS == true)
            {
                int statusPOS = (int)getPOS.GetValue("PortableOperatingSystem");
                if (statusPOS == 1)                                            //PortableOS Check
                {
                    Console.WriteLine("  WindowsToGo Features: Enabled");
                }
                else if (statusPOS == 0)
                {
                    Console.WriteLine("  WindowsToGo Features: Disabled");
                }
                else
                {
                    Console.WriteLine("  WindowsToGo Features: Status unknown");
                }
            }
            else
            {
                Console.WriteLine("  WindowsToGo Feature: [ERR: The specified registry key does not exist]");
            }
            getPOS.Close();
        }

        internal static void GetPartmgrSettings()
        {
            RegistryKey getPMGR = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\partmgr\\Parameters");
            int statusPMGR = (int)getPMGR.GetValue("SanPolicy");
            if (statusPMGR == 4)                                            //Partmgr Check
            {
                Console.WriteLine("  Hide Local Disks: True");
            }
            else
            {
                Console.WriteLine("  Hide Local Disks: False");
            }
            getPMGR.Close();
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
