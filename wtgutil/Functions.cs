using Microsoft.Win32;
using System;
using System.Linq;
using System.Security.Principal;
using static WTG_Utility_DeviceInfo;

namespace WTG_Utility.Functions
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
                    Console.WriteLine("Need to elevate privileges to run wtgutil.");
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
            catch (Exception ex)
            {
                Console.WriteLine($"  Boot from USB Devices: error occurred: {ex.Message}");
            }
        }

        internal static void GetPortableOSFeature()
        {
            try
            {
                RegistryKey getPOS = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
                bool existPOS = (getPOS.GetValueNames().Contains("PortableOperatingSystem"));
                int statusPOS = (int)getPOS.GetValue("PortableOperatingSystem");
                if (statusPOS == 1)                                            //PortableOS Check
                {
                    Console.WriteLine("  WindowsToGo Features:  Enabled");
                }
                else if (statusPOS == 0)
                {
                    Console.WriteLine("  WindowsToGo Features:  Disabled");
                }
                else
                {
                    Console.WriteLine("  WindowsToGo Features:  Status unknown");
                }
                getPOS.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  WindowsToGo Features:  error occurred: {ex.Message}");
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
                    Console.WriteLine("  Hide Local Disks:      True");
                }
                else
                {
                    Console.WriteLine("  Hide Local Disks:      False");
                }
                getPMGR.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Hide Local Disks:      error occurred: {ex.Message}");
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
                        Console.WriteLine("  UASP Status:           Disabled with UASPStor service modified");
                    }
                    else
                    {
                        Console.WriteLine("  UASP Status:           Disabled");
                    }
                }
                else if ((int)getUASP.GetValue("Capabilities") == 0x00000094
                    || (string)getUASP.GetValue("DeviceDesc") == "@usbstor.inf,%genericbulkonly.devicedesc%;USB Mass Storage Device"
                    || (string)getUASP.GetValue("Mfg") == "@usbstor.inf,%generic.mfg%;Compatible USB storage device"
                    || (string)getUASP.GetValue("Service") == "USBSTOR")
                {
                    Console.WriteLine("  UASP Status:           Unknown");
                }
                else
                {
                    if ((string)getUASPDriver.GetValue("ImagePath") == "\\SystemRoot\\System32\\drivers\\USBSTOR.SYS")
                    {
                        Console.WriteLine("  UASP Status:           Enabled with UASPStor service modified");
                    }
                    else
                    {
                        Console.WriteLine("  UASP Status:           Enabled");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(NullReferenceException))
                {
                    Console.WriteLine($"  UASP Status:           error occurred. Please ensure that the WTG drive is plugged in.");
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
                Console.WriteLine($"An error occured while getting device instance path: {ex.Message}");
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
                Console.WriteLine($"An error occured while modifying BootDriverFlags: {ex.Message}");
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
                Console.WriteLine($"An error occured while modifying portable operating system features: {ex.Message}");
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
                Console.WriteLine($"An error occured while modifying partmgr sanpolicy: {ex.Message}");
            }
        }

        internal static void DisableUASP(string deviceInstancePath)
        {
            try
            {
                RegistryKey disableUASP = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Enum\\"+deviceInstancePath);
                disableUASP.SetValue("Capabilities", 0x00000094, RegistryValueKind.DWord);
                disableUASP.SetValue("DeviceDesc", "@usbstor.inf,%genericbulkonly.devicedesc%;USB Mass Storage Device", RegistryValueKind.String);
                disableUASP.SetValue("Mfg", "@usbstor.inf,%generic.mfg%;Compatible USB storage device", RegistryValueKind.String);
                disableUASP.SetValue("Service", "USBSTOR", RegistryValueKind.String);
                disableUASP.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured while disabling UASP: {ex.Message}");
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
                Console.WriteLine($"An error occured while disabling UASP by driver config: {ex.Message}");
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
                Console.WriteLine($"An error occured while restoring UASP driver config: {ex.Message}");
            }
        }
    }
}
