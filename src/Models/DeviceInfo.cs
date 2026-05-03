namespace WTGUtility.Models
{
    /// <summary>
    /// Information about a detected WTG storage device.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>The device instance path (used for registry access).</summary>
        public string InstancePath { get; set; } = string.Empty;

        /// <summary>The controller DeviceID from WMI.</summary>
        public string ControllerDeviceId { get; set; } = string.Empty;

        /// <summary>Whether a WTG-compatible device was actually found.</summary>
        public bool IsFound => !string.IsNullOrEmpty(InstancePath);
    }
}
