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
                var partitionQuery = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_DiskPartition WHERE BootPartition = TRUE");

                foreach (ManagementObject partition in partitionQuery.Get())
                {
                    try
                    {
                        var driveQuery = new ManagementObjectSearcher(
                            $"ASSOCIATORS OF {{{partition.Path.Path}}} WHERE ResultClass = Win32_DiskDrive");

                        foreach (ManagementObject drive in driveQuery.Get())
                        {
                            string pnpId = drive["PNPDeviceID"]?.ToString() ?? "";

                            // Direct USB mass storage
                            if (pnpId.StartsWith("USB", StringComparison.OrdinalIgnoreCase))
                                return "USB";

                            // UASP: disk appears as SCSI — check if its controller is USB-attached
                            if (pnpId.StartsWith("SCSI", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    var ctrlQuery = new ManagementObjectSearcher(
                                        $"ASSOCIATORS OF {{{drive.Path.Path}}} WHERE ResultClass = Win32_SCSIController");
                                    foreach (ManagementObject ctrl in ctrlQuery.Get())
                                    {
                                        string ctrlPnp = ctrl["PNPDeviceID"]?.ToString() ?? "";
                                        string mfg = ctrl["Manufacturer"]?.ToString() ?? "";
                                        if (ctrlPnp.IndexOf("USB", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                            mfg.IndexOf("USB", StringComparison.OrdinalIgnoreCase) >= 0)
                                            return "SCSI";
                                    }
                                }
                                catch (ManagementException) { /* skip controller check */ }
                            }
                        }
                    }
                    catch (ManagementException) { /* skip this partition */ }
                }
            }
            catch (ManagementException) { /* WMI query failed */ }
            catch (Exception) { /* unexpected error */ }

            return "";
        }
    }
}
