using System;
using System.Threading.Tasks;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class ModeCommand : ICommand
    {
        public string Name => "mode";
        public string Description => Loc.Get("Cmd_Mode_Desc");
        public string SubCommandHelp => Loc.Get("Cmd_Mode_Help");

        public Task<int> ExecuteAsync(string[] args)
        {
            return Task.FromResult(0);
        }

        public static int Execute(CommandContext ctx, string subCommand)
        {
            ConsoleOutput.WriteBanner(Loc.Get("App_Title"), Loc.Format("App_Version", Loc.Version));
            ConsoleOutput.WriteSeparator();

            switch (subCommand)
            {
                case "wintogo":
                    ctx.WtgService.EnableWinToGoMode();
                    ConsoleOutput.WriteLine(Loc.Get("Msg_RestartNeeded"));
                    break;

                case "default":
                    ctx.WtgService.RestoreDefaults();
                    ConsoleOutput.WriteLine(Loc.Get("Msg_RestartNeeded"));
                    ConsoleOutput.WriteWarning(Loc.Get("Msg_WarningNoUSBoot"));
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
