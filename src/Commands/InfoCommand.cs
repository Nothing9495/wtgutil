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

            // Running on WTG drive?
            string bootType = ctx.WtgService.GetBootDriveType();
            string bootDriveType = bootType switch
            {
                "USB" => Loc.Get("BootStatus_WtgUSB"),
                "SCSI" => Loc.Get("BootStatus_WtgSCSI"),
                _ => Loc.Get("BootStatus_Local")
            };
            Console.WriteLine(Loc.Format("Info_BootDrive", bootDriveType));

            // BootDriverFlags
            string bootStatus = settings.BootDriverFlags switch
            {
                20 or 28 => Loc.Get("Status_Enabled"),
                0 => Loc.Get("Status_Disabled"),
                _ => Loc.Get("Status_Unknown")
            };
            Console.WriteLine(Loc.Format("Info_BootFromUSB", bootStatus));

            // PortableOS
            if (settings.WindowsToGoExists)
            {
                string wtgStatus = settings.WindowsToGoEnabled
                    ? Loc.Get("Status_Enabled")
                    : Loc.Get("Status_Disabled");
                Console.WriteLine(Loc.Format("Info_WTGFlag", wtgStatus));
            }
            else
            {
                Console.WriteLine(Loc.Format("Info_WTGFlag", Loc.Get("Status_Unknown")));
            }

            // Partmgr
            string hideStatus = settings.HideLocalDisks
                ? Loc.Get("HideStatus_True")
                : Loc.Get("HideStatus_False");
            Console.WriteLine(Loc.Format("Info_HideDisks", hideStatus));

            // UASP
            // Override display for local(fixed) boot drives
            string uaspDisplay = bootType == ""
                ? Loc.Get("Status_UaspUnavailable")
                : settings.UaspStatusDescription switch
                {
                    "Disabled" => Loc.Get("Status_Disabled"),
                    "Enabled" => Loc.Get("Status_Enabled"),
                    "NoDevice" => Loc.Get("Status_UaspNoDevice"),
                    _ => Loc.Get("Status_Unknown")
                };
            Console.WriteLine(Loc.Format("Info_UaspStatus", uaspDisplay));

            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Msg_Completed"));
            return 0;
        }
    }
}
