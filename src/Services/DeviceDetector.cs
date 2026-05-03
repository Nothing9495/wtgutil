using System;
using System.Management;
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
                var controllerQuery = new ManagementObjectSearcher(
                    @"SELECT * FROM Win32_SCSIController 
                      WHERE Manufacturer LIKE '%USB%' 
                      OR Description LIKE '%SCSI%'");

                foreach (ManagementObject controller in controllerQuery.Get())
                {
                    try
                    {
                        var controllerPath = controller.Path.Path;

                        var deviceQuery = new ManagementObjectSearcher($@"
                            ASSOCIATORS OF {{{controllerPath}}}
                            WHERE ResultClass = Win32_PnPEntity");

                        foreach (ManagementObject device in deviceQuery.Get())
                        {
                            try
                            {
                                var deviceId = device["DeviceID"]?.ToString();

                                if (!string.IsNullOrEmpty(deviceId) &&
                                    deviceId.StartsWith("USB", StringComparison.OrdinalIgnoreCase))
                                {
                                    return new DeviceInfo
                                    {
                                        InstancePath = controller["DeviceID"]?.ToString() ?? "",
                                        ControllerDeviceId = deviceId
                                    };
                                }
                            }
                            catch (ManagementException) { /* skip this device */ }
                        }
                    }
                    catch (ManagementException) { /* skip this controller */ }
                }
            }
            catch (ManagementException) { /* WMI query failed */ }
            catch (Exception) { /* unexpected error */ }

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

                            Infrastructure.ConsoleOutput.WriteDebug(
                                $"System disk: Model={model}, Interface={interfaceType}, Media={mediaType}, PnP={pnpId}");

                            // Direct USB mass storage
                            if (pnpId.StartsWith("USB", StringComparison.OrdinalIgnoreCase))
                            {
                                Infrastructure.ConsoleOutput.WriteDebug("  => Matched: direct USB mass storage");
                                return "USB";
                            }

                            // Fixed hard disk media → definitely internal (NVMe / SATA / SAS)
                            if (mediaType.IndexOf("Fixed", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                Infrastructure.ConsoleOutput.WriteDebug(
                                    $"  => Fixed media → local disk");
                                return "";
                            }

                            // External or Removable media on SCSI bus → UASP USB
                            if (mediaType.IndexOf("External", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                mediaType.IndexOf("Removable", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                Infrastructure.ConsoleOutput.WriteDebug("  => External/Removable media → UASP USB (SCSI)");
                                return "SCSI";
                            }

                            // Fallback: check interface type
                            if (interfaceType.IndexOf("USB", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                Infrastructure.ConsoleOutput.WriteDebug("  => USB interface type");
                                return "USB";
                            }

                            Infrastructure.ConsoleOutput.WriteDebug(
                                $"  => Unknown type: Media=\"{mediaType}\", falling back to local");
                        }
                    }
                }
            }
            catch (ManagementException ex)
            {
                Infrastructure.ConsoleOutput.WriteDebug("WMI error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Infrastructure.ConsoleOutput.WriteDebug("Unexpected error: " + ex.Message);
            }

            Infrastructure.ConsoleOutput.WriteDebug("=> Not a WTG boot drive");
            return "";
        }
    }
}
