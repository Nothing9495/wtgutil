namespace WTGUtility.Models
{
    /// <summary>
    /// Represents the current state of all WTG-related registry settings.
    /// </summary>
    public class WtgSettings
    {
        /// <summary>BootDriverFlags value from HKLM\SYSTEM\HardwareConfig\Current</summary>
        public int BootDriverFlags { get; set; }

        /// <summary>Whether PortableOperatingSystem exists and equals 1</summary>
        public bool WindowsToGoEnabled { get; set; }

        /// <summary>Whether PortableOperatingSystem registry value exists</summary>
        public bool WindowsToGoExists { get; set; }

        /// <summary>PortableOperatingSystem raw value</summary>
        public int PortableOperatingSystem { get; set; }

        /// <summary>Whether SanPolicy == 4 (hide local disks)</summary>
        public bool HideLocalDisks { get; set; }

        /// <summary>SanPolicy raw value</summary>
        public int SanPolicy { get; set; }

        /// <summary>Whether UASP is currently disabled for the WTG drive</summary>
        public bool UaspDisabled { get; set; }

        /// <summary>Human-readable UASP status description</summary>
        public string UaspStatusDescription { get; set; } = string.Empty;
    }
}
