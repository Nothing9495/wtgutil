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
    }
}
