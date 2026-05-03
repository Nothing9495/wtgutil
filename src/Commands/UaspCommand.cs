using System;
using System.Threading.Tasks;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class UaspCommand : ICommand
    {
        public string Name => "uasp";
        public string Description => Loc.Get("Cmd_Uasp_Desc");
        public string SubCommandHelp => Loc.Get("Cmd_Uasp_Help");

        public Task<int> ExecuteAsync(string[] args)
        {
            return Task.FromResult(0);
        }

        public static int Execute(CommandContext ctx, string subCommand)
        {
            if (string.IsNullOrEmpty(ctx.WtgDeviceInstancePath))
            {
                ConsoleOutput.WriteSeparator();
                ConsoleOutput.WriteError(Loc.Get("Error_NoWTGDrive"));
                return 1;
            }

            ConsoleOutput.WriteBanner(Loc.Get("App_Title"), Loc.Format("App_Version", Loc.Version));
            ConsoleOutput.WriteSeparator();

            switch (subCommand)
            {
                case "off":
                    ctx.WtgService.DisableUasp(ctx.WtgDeviceInstancePath);
                    break;

                case "on":
                    ctx.WtgService.EnableUasp(ctx.WtgDeviceInstancePath);
                    break;

                default:
                    ConsoleOutput.WriteError(Loc.Get("Error_UnknownParameter"));
                    return 1;
            }

            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Msg_Completed"));
            return 0;
        }
    }
}
