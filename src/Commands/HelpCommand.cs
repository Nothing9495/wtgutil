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
            var groups = GetCommandGroups();
            var lines = new List<string>();

            lines.Add(Loc.Get("Help_Header"));

            foreach (var group in groups)
            {
                foreach (var cmd in group)
                {
                    if (cmd.SubCommandHelp.Length > 0)
                    {
                        lines.Add($"  {cmd.Name,-22}{cmd.Description}");
                        foreach (var subLine in cmd.SubCommandHelp.Split('\n'))
                            lines.Add($"   {subLine}");
                    }
                    else
                    {
                        lines.Add($"  {cmd.Name,-22}{cmd.Description}");
                    }
                }
                lines.Add(""); // blank line between groups
            }

            lines.Add(Loc.Get("Help_GlobalOptions"));
            lines.Add("");
            lines.Add(Loc.Get("Help_Examples"));

            return string.Join("\n", lines);
        }

        /// <summary>
        /// Commands grouped by category, separated by blank lines in output.
        /// </summary>
        private static ICommand[][] GetCommandGroups()
        {
            return new ICommand[][]
            {
                new ICommand[] { new InfoCommand(), new HelpCommand(), new AboutCommand() },
                new ICommand[] { new InstallCommand(), new UninstallCommand() },
                new ICommand[] { new ModeCommand(), new PartmgrCommand(), new UaspCommand() },
            };
        }
    }
}
