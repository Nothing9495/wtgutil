using System;
using System.Management;
using WTGUtility.Infrastructure;
using WTGUtility.Models;

namespace WTGUtility.Services
{
    /// <summary>
    /// Detects the Windows To Go USB storage device via WMI.
    /// </summary>
    public class DeviceDetector
    {
        /// <summary>
        /// Finds the device instance path of the WTG SCSI/USB storage controller.
        /// Returns a DeviceInfo with IsFound=false if no suitable device is found.
        /// </summary>
        public DeviceInfo DetectWtgDevice()
        {
            try
            {
                ConsoleOutput.WriteDebug("WMI: Querying Win32_SCSIController for USB/SCSI devices...");
                var controllerQuery = new ManagementObjectSearcher(
                    @"SELECT * FROM Win32_SCSIController 
                      WHERE Manufacturer LIKE '%USB%' 
                      OR Description LIKE '%SCSI%'");

                foreach (ManagementObject controller in controllerQuery.Get())
                {
                    try
                    {
                        var controllerPath = controller.Path.Path;
                        var controllerDeviceId = controller["DeviceID"]?.ToString() ?? "";
                        ConsoleOutput.WriteDebug(
                            $"WMI: Found SCSI controller DeviceID=\"{controllerDeviceId}\"");

                        var deviceQuery = new ManagementObjectSearcher($@"
                            ASSOCIATORS OF {{{controllerPath}}}
                            WHERE ResultClass = Win32_PnPEntity");

                        foreach (ManagementObject device in deviceQuery.Get())
                        {
                            try
                            {
                                var deviceId = device["DeviceID"]?.ToString();
                                ConsoleOutput.WriteDebug(
                                    $"WMI:   Associated PnP device DeviceID=\"{deviceId}\"");

                                if (!string.IsNullOrEmpty(deviceId) &&
                                    deviceId.StartsWith("USB", StringComparison.OrdinalIgnoreCase))
                                {
                                    ConsoleOutput.WriteDebug(
                                        $"WMI:   => Matched USB device! InstancePath=\"{controllerDeviceId}\"");
                                    return new DeviceInfo
                                    {
                                        InstancePath = controllerDeviceId,
                                        ControllerDeviceId = deviceId
                                    };
                                }
                            }
                            catch (ManagementException ex)
                            {
                                ConsoleOutput.WriteDebug($"WMI:   PnP device query error: {ex.Message}");
                            }
                        }
                    }
                    catch (ManagementException ex)
                    {
                        ConsoleOutput.WriteDebug($"WMI: Controller association error: {ex.Message}");
                    }
                }
            }
            catch (ManagementException ex)
            {
                ConsoleOutput.WriteDebug($"WMI: SCSI controller query failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                ConsoleOutput.WriteDebug($"WMI: Unexpected error in DetectWtgDevice: {ex.Message}");
            }

            ConsoleOutput.WriteDebug("WMI: No WTG USB device found");
            return new DeviceInfo(); // IsFound will be false
        }

        /// <summary>
        /// Checks whether the current system is booted from a USB drive (i.e. a Windows To Go installation)
        /// by tracing the boot partition back to its physical disk via WMI.
        /// Returns "USB" for direct USB mass storage, "SCSI" for UASP bridge, or empty string for local boot.
        /// </summary>
        public string GetBootDriveType()
        {
            try
            {
                // Trace from the system drive (C:) to its physical disk — this
                // gives us exactly one disk, unlike BootPartition which may match many.
                var logicalQuery = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_LogicalDisk WHERE DeviceID = 'C:'");

                foreach (ManagementObject logical in logicalQuery.Get())
                {
                    var partitionQuery = new ManagementObjectSearcher(
                        $"ASSOCIATORS OF {{{logical.Path.Path}}} WHERE ResultClass = Win32_DiskPartition");

                    foreach (ManagementObject partition in partitionQuery.Get())
                    {
                        var driveQuery = new ManagementObjectSearcher(
                            $"ASSOCIATORS OF {{{partition.Path.Path}}} WHERE ResultClass = Win32_DiskDrive");

                        foreach (ManagementObject drive in driveQuery.Get())
                        {
                            string pnpId = drive["PNPDeviceID"]?.ToString() ?? "";
                            string model = drive["Model"]?.ToString() ?? "";
                            string interfaceType = drive["InterfaceType"]?.ToString() ?? "";
                            string mediaType = drive["MediaType"]?.ToString() ?? "";

                            ConsoleOutput.WriteDebug(
                                $"System disk: Model={model}, Interface={interfaceType}, Media={mediaType}, PnP={pnpId}");

                            // Direct USB mass storage
                            if (pnpId.StartsWith("USB", StringComparison.OrdinalIgnoreCase))
                            {
                                ConsoleOutput.WriteDebug("  => Matched: direct USB mass storage");
                                return "USB";
                            }

                            // Fixed hard disk media → definitely internal (NVMe / SATA / SAS)
                            if (mediaType.IndexOf("Fixed", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                ConsoleOutput.WriteDebug(
                                    $"  => Fixed media → local disk");
                                return "";
                            }

                            // External or Removable media on SCSI bus → UASP USB
                            if (mediaType.IndexOf("External", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                mediaType.IndexOf("Removable", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                ConsoleOutput.WriteDebug("  => External/Removable media → UASP USB (SCSI)");
                                return "SCSI";
                            }

                            // Fallback: check interface type
                            if (interfaceType.IndexOf("USB", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                ConsoleOutput.WriteDebug("  => USB interface type");
                                return "USB";
                            }

                            ConsoleOutput.WriteDebug(
                                $"  => Unknown type: Media=\"{mediaType}\", falling back to local");
                        }
                    }
                }
            }
            catch (ManagementException ex)
            {
                ConsoleOutput.WriteDebug("WMI error: " + ex.Message);
            }
            catch (Exception ex)
            {
                ConsoleOutput.WriteDebug("Unexpected error: " + ex.Message);
            }

            ConsoleOutput.WriteDebug("=> Not a WinToGo boot drive");
            return "";
        }
    }
}
