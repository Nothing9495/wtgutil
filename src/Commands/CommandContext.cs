using WTGUtility.Services;

namespace WTGUtility.Commands
{
    /// <summary>
    /// Shared context passed to all commands.
    /// </summary>
    public class CommandContext
    {
        public WtgService WtgService { get; }
        public string WtgDeviceInstancePath { get; set; } = string.Empty;

        public CommandContext(WtgService wtgService)
        {
            WtgService = wtgService;
        }
    }
}
