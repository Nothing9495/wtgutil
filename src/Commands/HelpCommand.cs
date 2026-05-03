using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name => "help";
        public string Description => Loc.Get("Cmd_Help_Desc");
        public string SubCommandHelp => string.Empty;

        public Task<int> ExecuteAsync(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine(GenerateHelpText());
            Console.WriteLine();
            return Task.FromResult(0);
        }

        /// <summary>
        /// Dynamically builds help text from all registered command definitions.
        /// </summary>
        public static string GenerateHelpText()
        {
            var commands = GetAllCommands();
            var lines = new List<string>();

            lines.Add(Loc.Get("Help_Header"));

            foreach (var cmd in commands)
            {
                if (cmd.SubCommandHelp.Length > 0)
                {
                    lines.Add($"  {cmd.Name,-22}{cmd.Description}");
                    foreach (var subLine in cmd.SubCommandHelp.Split('\n'))
                        lines.Add($"    {subLine}");
                }
                else
                {
                    lines.Add($"  {cmd.Name,-22}{cmd.Description}");
                }
            }

            lines.Add("");
            lines.Add(Loc.Get("Help_GlobalOptions"));
            lines.Add("");
            lines.Add(Loc.Get("Help_Examples"));

            return string.Join("\n", lines);
        }

        private static ICommand[] GetAllCommands()
        {
            return new ICommand[]
            {
                new InfoCommand(),
                new HelpCommand(),
                new AboutCommand(),
                new ModeCommand(),
                new PartmgrCommand(),
                new UaspCommand(),
            };
        }
    }
}
