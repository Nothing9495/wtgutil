using Microsoft.Win32;
using System;

namespace WTG_Utility.Functions
{

    internal class GetSettings
    {
        internal static void CurrentInfo()
        {
            Console.WriteLine("Current Info:");
        }

        internal static void GetBootDriverFlags()
        {
            RegistryKey getBDF = Registry.LocalMachine.OpenSubKey("SYSTEM\\HardwareConfig\\Current");
            int statusBDF = (int)getBDF.GetValue("BootDriverFlags");
            getBDF.Close();

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
        }

        internal static void GetPortableOSFeature()
        {
            RegistryKey getPOS = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
            int statusPOS = (int)getPOS.GetValue("PortableOperatingSystem");
            getPOS.Close();

            if (statusPOS == 1)                                            //PortableOS Check
            {
                Console.WriteLine("  PortableOS Feature: Enabled");
            }
            else if (statusPOS == 0)
            {
                Console.WriteLine("  PortableOS Feature: Disabled");
            }
            else
            {
                Console.WriteLine("  PortableOS Feature: Status unknown");
            }
        }

        internal static void GetPartmgrSettings()
        {
            RegistryKey getPMGR = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\partmgr\\Parameters");
            int statusPMGR = (int)getPMGR.GetValue("SanPolicy");
            getPMGR.Close();

            if (statusPMGR == 4)                                            //Partmgr Check
            {
                Console.WriteLine("  Hide Local Disks: True");
            }
            else
            {
                Console.WriteLine("  Hide Local Disks: False");
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
