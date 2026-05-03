using System;
using System.Threading.Tasks;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class PartmgrCommand : ICommand
    {
        public string Name => "partmgr";
        public string Description => Loc.Get("Cmd_Partmgr_Desc");
        public string SubCommandHelp => Loc.Get("Cmd_Partmgr_Help");

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
                case "show":
                    ctx.WtgService.ShowLocalDisks();
                    break;

                case "hide":
                    ctx.WtgService.HideLocalDisks();
                    break;

                default:
                    ConsoleOutput.WriteError(Loc.Get("Error_UnknownParameter"));
                    return 1;
            }

            ConsoleOutput.WriteLine(Loc.Get("Msg_RestartNeeded"));
            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Msg_Completed"));
            return 0;
        }
    }
}
