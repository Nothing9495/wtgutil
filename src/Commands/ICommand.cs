using System.Threading.Tasks;

namespace WTGUtility.Commands
{
    /// <summary>
    /// Contract for a CLI command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>The primary command name (lowercase).</summary>
        string Name { get; }

        /// <summary>Localized one-line description shown in help.</summary>
        string Description { get; }

        /// <summary>Localized subcommand usage detail (empty for simple commands).</summary>
        string SubCommandHelp { get; }

        /// <summary>Executes the command with the given arguments.</summary>
        /// <returns>Exit code: 0 = success, non-zero = error.</returns>
        Task<int> ExecuteAsync(string[] args);
    }
}
