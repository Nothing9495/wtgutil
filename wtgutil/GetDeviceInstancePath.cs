using System;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

public class WTG_Utility_DeviceInfo
{

    public static string FindScsiStorageDevices()
    {
        try
        {
            // 精确查询SCSI控制器（包含USB-SCSI桥接器）
            var controllerQuery = new ManagementObjectSearcher(@"
                SELECT * FROM Win32_SCSIController 
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

                    bool usbDeviceFound = false;

                    foreach (ManagementObject device in deviceQuery.Get())
                    {
                        try
                        {
                            var hardwareIds = device["HardwareID"] as string[];
                            var deviceId = device["DeviceID"]?.ToString();

                            // USB设备优先匹配逻辑
                            if (!string.IsNullOrEmpty(deviceId) &&
                                deviceId.StartsWith("USB", StringComparison.OrdinalIgnoreCase))
                            {
                                usbDeviceFound = true;
                                break; // 跳出设备循环
                            }

                            // 原有SCSI设备验证
                            if (IsValidScsiStorage(deviceId, hardwareIds))
                            {
                                // 处理SCSI设备逻辑
                            }
                        }
                        catch (ManagementException ex)
                        {
                            Console.WriteLine($"设备查询失败: {ex.Message}");
                        }
                    }

                    if (usbDeviceFound)
                    {
                        return controller["DeviceID"]?.ToString(); // 返回控制器设备ID
                    }
                }
                catch (ManagementException ex)
                {
                    Console.WriteLine($"控制器 {controller["DeviceID"]} 查询失败: {ex.Message}");
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"查询SCSI控制器失败: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生未知错误: {ex.Message}");
        }
        return null; // 如果没有找到合适的设备，返回null
    }

    private static bool IsValidScsiStorage(string deviceId, string[] hardwareIds)
    {
        // 特征1：设备路径必须包含SCSI标识
        bool isScsiPath = (deviceId?.StartsWith("SCSI", StringComparison.OrdinalIgnoreCase) == true) ||
                          (deviceId?.Contains("USBSTOR") == true);  // 兼容USB-SCSI桥接

        // 特征2：硬件ID必须符合存储设备特征
        bool hasValidHardwareId = hardwareIds?.Any(id =>
            id.StartsWith("SCSI\\", StringComparison.OrdinalIgnoreCase) ||
            id.StartsWith("USBSTOR\\", StringComparison.OrdinalIgnoreCase)
        ) ?? false;

        return isScsiPath && hasValidHardwareId;
    }
}

//public class DeviceInfo
//{
//    public string DeviceId { get; }
//    public string ControllerPath { get; }
//    public string[] HardwareIds { get; }

//    public DeviceInfo(string deviceId, string controllerPath, string[] hardwareIds)
//    {
//        DeviceId = deviceId;
//        ControllerPath = controllerPath;
//        HardwareIds = hardwareIds;
//    }
//}
