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

        public void EnableWinToGoMode()
        {
            _registry.SetBootDriverFlags(20);
            _registry.SetPortableOperatingSystem(1);
            _registry.SetSanPolicy(4);
        }

        public void RestoreDefaults()
        {
            _registry.SetBootDriverFlags(0);
            _registry.SetPortableOperatingSystem(0);
            _registry.SetSanPolicy(1);
        }

        public void ShowLocalDisks()
        {
            _registry.SetSanPolicy(1);
        }

        public void HideLocalDisks()
        {
            _registry.SetSanPolicy(4);
        }

        public void DisableUasp(string deviceInstancePath)
        {
            _registry.DisableUasp(deviceInstancePath);
        }

        public void EnableUasp(string deviceInstancePath)
        {
            _registry.EnableUasp(deviceInstancePath);
        }
    }
}
