using System;
using WTGUtility.Commands;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;
using WTGUtility.Services;

namespace WTGUtility
{
    class Program
    {
        static int Main(string[] args)
        {
            // 1. Check administrator privileges (runs as invoker; exits if not admin)
            AdminCheck.EnsureAdministrator();

            // 2. Set console title
            Console.Title = Loc.Get("App_ShortTitle");

            // 3. Initialize services (Pure DI)
            var registry = new RegistryService();
            var detector = new DeviceDetector();
            var wtgService = new WtgService(registry, detector);

            // 4. Detect WTG device
            var deviceInfo = detector.DetectWtgDevice();

            // 5. Build command context
            var context = new CommandContext(wtgService)
            {
                WtgDeviceInstancePath = deviceInfo.InstancePath
            };

            // 6. Route and execute
            var router = new CommandRouter(context);
            return router.Route(args);
        }
    }
}
