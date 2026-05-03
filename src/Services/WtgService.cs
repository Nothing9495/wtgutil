using WTGUtility.Infrastructure;
using WTGUtility.Models;

namespace WTGUtility.Services
{
    /// <summary>
    /// High-level Windows To Go business logic.
    /// Orchestrates registry operations and device detection.
    /// </summary>
    public class WtgService
    {
        private readonly RegistryService _registry;
        private readonly DeviceDetector _detector;

        public WtgService(RegistryService registry, DeviceDetector detector)
        {
            _registry = registry;
            _detector = detector;
        }

        public DeviceInfo DetectDevice()
        {
            return _detector.DetectWtgDevice();
        }

        public WtgSettings GetSettings(string deviceInstancePath)
        {
            return _registry.GetSettings(deviceInstancePath);
        }

        /// <summary>Returns the boot drive type: "USB", "SCSI" (UASP), or empty string for local boot.</summary>
        public string GetBootDriveType()
        {
            return _detector.GetBootDriveType();
        }

        public void EnableWinToGoMode()
        {
            ConsoleOutput.WriteDebug("WtgService: EnableWinToGoMode() — setting BootDriverFlags=20, PortableOperatingSystem=1, SanPolicy=4");
            _registry.SetBootDriverFlags(20);
            _registry.SetPortableOperatingSystem(1);
            _registry.SetSanPolicy(4);
        }

        public void RestoreDefaults()
        {
            ConsoleOutput.WriteDebug("WtgService: RestoreDefaults() — setting BootDriverFlags=0, PortableOperatingSystem=0, SanPolicy=1");
            _registry.SetBootDriverFlags(0);
            _registry.SetPortableOperatingSystem(0);
            _registry.SetSanPolicy(1);
        }

        public void ShowLocalDisks()
        {
            ConsoleOutput.WriteDebug("WtgService: ShowLocalDisks() — setting SanPolicy=1");
            _registry.SetSanPolicy(1);
        }

        public void HideLocalDisks()
        {
            ConsoleOutput.WriteDebug("WtgService: HideLocalDisks() — setting SanPolicy=4");
            _registry.SetSanPolicy(4);
        }

        public void DisableUasp(string deviceInstancePath)
        {
            ConsoleOutput.WriteDebug($"WtgService: DisableUasp(deviceInstancePath=\"{deviceInstancePath}\")");
            _registry.DisableUasp(deviceInstancePath);
        }

        public void EnableUasp(string deviceInstancePath)
        {
            ConsoleOutput.WriteDebug($"WtgService: EnableUasp(deviceInstancePath=\"{deviceInstancePath}\")");
            _registry.EnableUasp(deviceInstancePath);
        }
    }
}
