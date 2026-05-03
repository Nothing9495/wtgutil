using System;
using Microsoft.Win32;
using WTGUtility.Infrastructure;
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

            // --- BootDriverFlags ---
            try
            {
                using var bdfKey = Registry.LocalMachine.OpenSubKey(PathBootDriverFlags);
                if (bdfKey != null)
                {
                    settings.BootDriverFlags = (int)bdfKey.GetValue(ValBootDriverFlags, 0);
                    ConsoleOutput.WriteDebug(
                        $"RegRead: HKLM\\{PathBootDriverFlags}\\{ValBootDriverFlags} = {settings.BootDriverFlags}");
                }
                else
                {
                    settings.BootDriverFlags = -1;
                    ConsoleOutput.WriteDebug(
                        $"RegRead: HKLM\\{PathBootDriverFlags} key not found, {ValBootDriverFlags} = -1");
                }
            }
            catch (Exception ex)
            {
                settings.BootDriverFlags = -1;
                ConsoleOutput.WriteDebug(
                    $"RegRead: HKLM\\{PathBootDriverFlags}\\{ValBootDriverFlags} ERROR: {ex.Message}");
            }

            // --- PortableOperatingSystem ---
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
                    ConsoleOutput.WriteDebug(
                        $"RegRead: HKLM\\{PathPortableOS}\\{ValPortableOperatingSystem} = {settings.PortableOperatingSystem} (Exists={settings.WindowsToGoExists}, Enabled={settings.WindowsToGoEnabled})");
                }
                else
                {
                    ConsoleOutput.WriteDebug(
                        $"RegRead: HKLM\\{PathPortableOS} key not found");
                }
            }
            catch (Exception ex)
            {
                ConsoleOutput.WriteDebug(
                    $"RegRead: HKLM\\{PathPortableOS}\\{ValPortableOperatingSystem} ERROR: {ex.Message}");
            }

            // --- SanPolicy ---
            try
            {
                using var pmgrKey = Registry.LocalMachine.OpenSubKey(PathPartmgrParams);
                if (pmgrKey != null)
                {
                    settings.SanPolicy = (int)pmgrKey.GetValue(ValSanPolicy, 1);
                    settings.HideLocalDisks = settings.SanPolicy == 4;
                    ConsoleOutput.WriteDebug(
                        $"RegRead: HKLM\\{PathPartmgrParams}\\{ValSanPolicy} = {settings.SanPolicy} (HideLocalDisks={settings.HideLocalDisks})");
                }
                else
                {
                    ConsoleOutput.WriteDebug(
                        $"RegRead: HKLM\\{PathPartmgrParams} key not found");
                }
            }
            catch (Exception ex)
            {
                ConsoleOutput.WriteDebug(
                    $"RegRead: HKLM\\{PathPartmgrParams}\\{ValSanPolicy} ERROR: {ex.Message}");
            }

            // --- UASP status ---
            if (string.IsNullOrEmpty(deviceInstancePath))
            {
                settings.UaspStatusDescription = "NoDevice";
                ConsoleOutput.WriteDebug("UASP: No device instance path provided, status=NoDevice");
            }
            else
            {
                try
                {
                    string fullPath = PathEnumPrefix + deviceInstancePath;
                    using var uaspKey = Registry.LocalMachine.OpenSubKey(fullPath);
                    if (uaspKey != null)
                    {
                        int cap = (int)uaspKey.GetValue(ValCapabilities, 0);
                        string desc = uaspKey.GetValue(ValDeviceDesc, "") as string ?? "";
                        string mfg = uaspKey.GetValue(ValMfg, "") as string ?? "";
                        string svc = uaspKey.GetValue(ValService, "") as string ?? "";

                        ConsoleOutput.WriteDebug(
                            $"RegRead: HKLM\\{fullPath}\\{ValCapabilities} = 0x{cap:X8}");
                        ConsoleOutput.WriteDebug(
                            $"RegRead: HKLM\\{fullPath}\\{ValDeviceDesc} = \"{desc}\"");
                        ConsoleOutput.WriteDebug(
                            $"RegRead: HKLM\\{fullPath}\\{ValMfg} = \"{mfg}\"");
                        ConsoleOutput.WriteDebug(
                            $"RegRead: HKLM\\{fullPath}\\{ValService} = \"{svc}\"");

                        settings.UaspDisabled =
                            cap == UaspDisabledCapabilities &&
                            desc == UaspDisabledDeviceDesc &&
                            mfg == UaspDisabledMfg &&
                            svc == UaspDisabledService;
                        settings.UaspStatusDescription = settings.UaspDisabled ? "Disabled" : "Enabled";
                        ConsoleOutput.WriteDebug(
                            $"UASP status: {settings.UaspStatusDescription} (DevicePath={deviceInstancePath})");
                    }
                    else
                    {
                        ConsoleOutput.WriteDebug(
                            $"RegRead: HKLM\\{fullPath} key not found");
                    }
                }
                catch (Exception ex)
                {
                    settings.UaspStatusDescription = "Unknown";
                    ConsoleOutput.WriteDebug(
                        $"RegRead: UASP device key ERROR: {ex.Message}");
                }
            }

            return settings;
        }

        /// <summary>Sets BootDriverFlags (20 = enable USB boot, 0 = disable).</summary>
        public void SetBootDriverFlags(int value)
        {
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{PathBootDriverFlags}\\{ValBootDriverFlags} = {value}");
            using var key = Registry.LocalMachine.CreateSubKey(PathBootDriverFlags);
            key.SetValue(ValBootDriverFlags, value, RegistryValueKind.DWord);
        }

        /// <summary>Sets PortableOperatingSystem (1 = enabled, 0 = disabled).</summary>
        public void SetPortableOperatingSystem(int value)
        {
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{PathPortableOS}\\{ValPortableOperatingSystem} = {value}");
            using var key = Registry.LocalMachine.CreateSubKey(PathPortableOS);
            key.SetValue(ValPortableOperatingSystem, value, RegistryValueKind.DWord);
        }

        /// <summary>Sets SanPolicy (4 = hide, 1 = show).</summary>
        public void SetSanPolicy(int value)
        {
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{PathPartmgrParams}\\{ValSanPolicy} = {value}");
            using var key = Registry.LocalMachine.CreateSubKey(PathPartmgrParams);
            key.SetValue(ValSanPolicy, value, RegistryValueKind.DWord);
        }

        /// <summary>Disables UASP by modifying the device's registry entries.</summary>
        public void DisableUasp(string deviceInstancePath)
        {
            string fullPath = PathEnumPrefix + deviceInstancePath;
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{fullPath}\\{ValCapabilities} = 0x{UaspDisabledCapabilities:X8}");
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{fullPath}\\{ValDeviceDesc} = \"{UaspDisabledDeviceDesc}\"");
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{fullPath}\\{ValMfg} = \"{UaspDisabledMfg}\"");
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{fullPath}\\{ValService} = \"{UaspDisabledService}\"");
            using var key = Registry.LocalMachine.CreateSubKey(fullPath);
            key.SetValue(ValCapabilities, UaspDisabledCapabilities, RegistryValueKind.DWord);
            key.SetValue(ValDeviceDesc, UaspDisabledDeviceDesc, RegistryValueKind.String);
            key.SetValue(ValMfg, UaspDisabledMfg, RegistryValueKind.String);
            key.SetValue(ValService, UaspDisabledService, RegistryValueKind.String);
        }

        /// <summary>Re-enables UASP by reverting device registry entries to defaults.</summary>
        public void EnableUasp(string deviceInstancePath)
        {
            string fullPath = PathEnumPrefix + deviceInstancePath;
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{fullPath}\\{ValCapabilities} = 0x{DefaultCapabilities:X8}");
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{fullPath}\\{ValDeviceDesc} = \"{DefaultDeviceDesc}\"");
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{fullPath}\\{ValMfg} = \"{DefaultMfg}\"");
            ConsoleOutput.WriteDebug(
                $"RegWrite: HKLM\\{fullPath}\\{ValService} = \"{DefaultService}\"");
            using var key = Registry.LocalMachine.CreateSubKey(fullPath);
            key.SetValue(ValCapabilities, DefaultCapabilities, RegistryValueKind.DWord);
            key.SetValue(ValDeviceDesc, DefaultDeviceDesc, RegistryValueKind.String);
            key.SetValue(ValMfg, DefaultMfg, RegistryValueKind.String);
            key.SetValue(ValService, DefaultService, RegistryValueKind.String);
        }
    }
}
