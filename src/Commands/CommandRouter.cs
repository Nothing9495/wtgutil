using System;
using System.Collections.Generic;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    /// <summary>
    /// Routes CLI arguments to the appropriate command handler.
    /// Supports --lang, --help, --version global options.
    /// </summary>
    public class CommandRouter
    {
        private readonly CommandContext _context;
        private readonly Dictionary<string, Func<string[], int>> _routes;

        public CommandRouter(CommandContext context)
        {
            _context = context;
            _routes = new Dictionary<string, Func<string[], int>>(StringComparer.OrdinalIgnoreCase)
            {
                ["info"]    = _ => RunInfo(),
                ["help"]    = _ => RunHelp(),
                ["about"]   = _ => RunAbout(),
                ["install"]   = _ => InstallCommand.Execute(_context),
                ["uninstall"] = _ => UninstallCommand.Execute(_context),
                ["mode"]    = args => RunWithSubCommand(args, ModeCommand.Execute),
                ["partmgr"] = args => RunWithSubCommand(args, PartmgrCommand.Execute),
                ["uasp"]    = args => RunWithSubCommand(args, UaspCommand.Execute),
            };
        }

        /// <summary>
        /// Parses global options and dispatches to the appropriate command.
        /// </summary>
        public int Route(string[] rawArgs)
        {
            var args = new List<string>(rawArgs);

            // Parse global options
            while (args.Count > 0 && args[0].StartsWith("--"))
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "--lang":
                        if (args.Count > 1)
                        {
                            try
                            {
                                Loc.Culture = new System.Globalization.CultureInfo(args[1]);
                            }
                            catch { /* ignore invalid culture */ }
                            args.RemoveAt(0);
                        }
                        args.RemoveAt(0);
                        break;

                    case "--help":
                        return RunHelp();

                    case "--version":
                        return RunVersion();

                    case "--debug":
                        _context.Debug = true;
                        ConsoleOutput.IsDebug = true;
                        args.RemoveAt(0);
                        break;

                    default:
                        ConsoleOutput.WriteError(Loc.Get("Error_UnknownCommand"));
                        return 1;
                }
            }

            if (args.Count == 0)
            {
                return RunHelp();
            }

            string command = args[0].ToLowerInvariant();
            args.RemoveAt(0);

            if (_routes.TryGetValue(command, out var handler))
            {
                try
                {
                    return handler(args.ToArray());
                }
                catch (Exception ex)
                {
                    ConsoleOutput.WriteError(Loc.Format("Error_Unexpected", ex.Message));
                    return 1;
                }
            }
            else
            {
                ConsoleOutput.WriteError(Loc.Get("Error_UnknownCommand"));
                return 1;
            }
        }
        private int RunWithSubCommand(string[] subArgs, Func<CommandContext, string, int> handler)
        {
            if (subArgs.Length == 0)
            {
                ConsoleOutput.WriteError(Loc.Get("Error_NoParameter"));
                return 1;
            }

            string sub = subArgs[0].ToLowerInvariant();
            return handler(_context, sub);
        }
        private int RunInfo()
        {
            try
            {
                return InfoCommand.Execute(_context);
            }
            catch (Exception ex)
            {
                ConsoleOutput.WriteError(ex.Message);
                return 1;
            }
        }

        private int RunHelp()
        {
            Console.WriteLine();
            Console.WriteLine(HelpCommand.GenerateHelpText());
            Console.WriteLine();
            return 0;
        }

        private int RunAbout()
        {
            Console.WriteLine();
            Console.WriteLine(Loc.Format("About_Text", Loc.Version));
            Console.WriteLine();
            return 0;
        }

        private int RunVersion()
        {
            Console.WriteLine(Loc.Format("Version_Text", Loc.Version));
            return 0;
        }
    }
}
