using System;
using System.Threading.Tasks;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class AboutCommand : ICommand
    {
        public string Name => "about";
        public string Description => Loc.Get("Cmd_About_Desc");
        public string SubCommandHelp => string.Empty;

        public Task<int> ExecuteAsync(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine(Loc.Format("About_Text", Loc.Version));
            Console.WriteLine();
            return Task.FromResult(0);
        }
    }
}
