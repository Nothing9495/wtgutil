using System;
using System.Threading.Tasks;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class InfoCommand : ICommand
    {
        public string Name => "info";
        public string Description => Loc.Get("Cmd_Info_Desc");
        public string SubCommandHelp => string.Empty;

        public Task<int> ExecuteAsync(string[] args)
        {
            return Task.FromResult(0);
        }

        /// <summary>Static entry called by CommandRouter with full context.</summary>
        public static int Execute(CommandContext ctx)
        {
            ConsoleOutput.WriteBanner(Loc.Get("App_Title"), Loc.Format("App_Version", Loc.Version));
            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Info_CurrentSettings"));

            var settings = ctx.WtgService.GetSettings(ctx.WtgDeviceInstancePath);

            // BootDriverFlags
            string bootMsg = settings.BootDriverFlags switch
            {
                20 or 28 => Loc.Get("Info_BootFromUSB_Enabled"),
                0 => Loc.Get("Info_BootFromUSB_Disabled"),
                _ => Loc.Get("Info_BootFromUSB_Unknown")
            };
            Console.WriteLine(bootMsg);

            // PortableOS
            if (settings.WindowsToGoExists)
            {
                Console.WriteLine(settings.WindowsToGoEnabled
                    ? Loc.Get("Info_WTG_Enabled")
                    : Loc.Get("Info_WTG_Disabled"));
            }
            else
            {
                Console.WriteLine(Loc.Get("Info_WTG_Unknown"));
            }

            // Partmgr
            Console.WriteLine(settings.HideLocalDisks
                ? Loc.Get("Info_HideDisks_True")
                : Loc.Get("Info_HideDisks_False"));

            // UASP
            string uaspMsg = settings.UaspStatusDescription switch
            {
                "Disabled" => Loc.Get("Info_UASP_Disabled"),
                "Enabled" => Loc.Get("Info_UASP_Enabled"),
                "NoDevice" => Loc.Get("Info_UASP_NoDevice"),
                _ => Loc.Get("Info_UASP_Unknown")
            };
            Console.WriteLine(uaspMsg);

            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Msg_Completed"));
            return 0;
        }
    }
}
