using System;
using Microsoft.Win32;
using WTGUtility.Models;

namespace WTGUtility.Services
{
    /// <summary>
    /// Centralized registry access for WTG settings.
    /// All registry paths and value names are defined as constants.
    /// </summary>
    public class RegistryService
    {
        // Registry paths
        private const string PathBootDriverFlags = @"SYSTEM\HardwareConfig\Current";
        private const string PathPortableOS = @"SYSTEM\CurrentControlSet\Control";
        private const string PathPartmgrParams = @"SYSTEM\CurrentControlSet\Services\partmgr\Parameters";
        private const string PathEnumPrefix = @"SYSTEM\CurrentControlSet\Enum\";

        // Value names
        private const string ValBootDriverFlags = "BootDriverFlags";
        private const string ValPortableOperatingSystem = "PortableOperatingSystem";
        private const string ValSanPolicy = "SanPolicy";
        private const string ValCapabilities = "Capabilities";
        private const string ValDeviceDesc = "DeviceDesc";
        private const string ValMfg = "Mfg";
        private const string ValService = "Service";

        // UASP state values
        private const int UaspDisabledCapabilities = 0x00000094;
        private const string UaspDisabledDeviceDesc = "@usbstor.inf,%genericbulkonly.devicedesc%;USB Mass Storage Device";
        private const string UaspDisabledMfg = "@usbstor.inf,%generic.mfg%;Compatible USB storage device";
        private const string UaspDisabledService = "USBSTOR";

        // Default (UASP enabled) values to restore
        private const int DefaultCapabilities = 0x00000094;
        private const string DefaultService = "UASPStor";
        private const string DefaultDeviceDesc = "@usbstor.inf,%genericbulkonly.devicedesc%;USB Mass Storage Device";
        private const string DefaultMfg = "@usbstor.inf,%generic.mfg%;Compatible USB storage device";

        /// <summary>
        /// Reads all current WTG settings from the registry.
        /// </summary>
        public WtgSettings GetSettings(string deviceInstancePath)
        {
            var settings = new WtgSettings();

            try
            {
                using var bdfKey = Registry.LocalMachine.OpenSubKey(PathBootDriverFlags);
                settings.BootDriverFlags = bdfKey != null ? (int)bdfKey.GetValue(ValBootDriverFlags, 0) : -1;
            }
            catch { settings.BootDriverFlags = -1; }

            try
            {
                using var posKey = Registry.LocalMachine.OpenSubKey(PathPortableOS);
                if (posKey != null)
                {
                    settings.WindowsToGoExists = posKey.GetValueNames().Length > 0 &&
                        Array.IndexOf(posKey.GetValueNames(), ValPortableOperatingSystem) >= 0;
                    if (settings.WindowsToGoExists)
                    {
                        settings.PortableOperatingSystem = (int)posKey.GetValue(ValPortableOperatingSystem, 0);
                        settings.WindowsToGoEnabled = settings.PortableOperatingSystem == 1;
                    }
                }
            }
            catch { /* leave defaults */ }

            try
            {
                using var pmgrKey = Registry.LocalMachine.OpenSubKey(PathPartmgrParams);
                if (pmgrKey != null)
                {
                    settings.SanPolicy = (int)pmgrKey.GetValue(ValSanPolicy, 1);
                    settings.HideLocalDisks = settings.SanPolicy == 4;
                }
            }
            catch { /* leave defaults */ }

            // UASP status
            if (string.IsNullOrEmpty(deviceInstancePath))
            {
                settings.UaspStatusDescription = "NoDevice";
            }
            else
            {
                try
                {
                    using var uaspKey = Registry.LocalMachine.OpenSubKey(PathEnumPrefix + deviceInstancePath);
                    if (uaspKey != null)
                    {
                        int cap = (int)uaspKey.GetValue(ValCapabilities, 0);
                        string desc = uaspKey.GetValue(ValDeviceDesc, "") as string ?? "";
                        string mfg = uaspKey.GetValue(ValMfg, "") as string ?? "";
                        string svc = uaspKey.GetValue(ValService, "") as string ?? "";

                        settings.UaspDisabled =
                            cap == UaspDisabledCapabilities &&
                            desc == UaspDisabledDeviceDesc &&
                            mfg == UaspDisabledMfg &&
                            svc == UaspDisabledService;
                        settings.UaspStatusDescription = settings.UaspDisabled ? "Disabled" : "Enabled";
                    }
                }
                catch
                {
                    settings.UaspStatusDescription = "Unknown";
                }
            }

            return settings;
        }

        /// <summary>Sets BootDriverFlags (20 = enable USB boot, 0 = disable).</summary>
        public void SetBootDriverFlags(int value)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathBootDriverFlags);
            key.SetValue(ValBootDriverFlags, value, RegistryValueKind.DWord);
        }

        /// <summary>Sets PortableOperatingSystem (1 = enabled, 0 = disabled).</summary>
        public void SetPortableOperatingSystem(int value)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathPortableOS);
            key.SetValue(ValPortableOperatingSystem, value, RegistryValueKind.DWord);
        }

        /// <summary>Sets SanPolicy (4 = hide, 1 = show).</summary>
        public void SetSanPolicy(int value)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathPartmgrParams);
            key.SetValue(ValSanPolicy, value, RegistryValueKind.DWord);
        }

        /// <summary>Disables UASP by modifying the device's registry entries.</summary>
        public void DisableUasp(string deviceInstancePath)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathEnumPrefix + deviceInstancePath);
            key.SetValue(ValCapabilities, UaspDisabledCapabilities, RegistryValueKind.DWord);
            key.SetValue(ValDeviceDesc, UaspDisabledDeviceDesc, RegistryValueKind.String);
            key.SetValue(ValMfg, UaspDisabledMfg, RegistryValueKind.String);
            key.SetValue(ValService, UaspDisabledService, RegistryValueKind.String);
        }

        /// <summary>Re-enables UASP by reverting device registry entries to defaults.</summary>
        public void EnableUasp(string deviceInstancePath)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathEnumPrefix + deviceInstancePath);
            key.SetValue(ValCapabilities, DefaultCapabilities, RegistryValueKind.DWord);
            key.SetValue(ValDeviceDesc, DefaultDeviceDesc, RegistryValueKind.String);
            key.SetValue(ValMfg, DefaultMfg, RegistryValueKind.String);
            key.SetValue(ValService, DefaultService, RegistryValueKind.String);
        }
    }
}
