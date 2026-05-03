using System;
using System.Security.Principal;
using WTGUtility.Localization;

namespace WTGUtility.Infrastructure
{
    /// <summary>
    /// Ensures the process has administrator privileges.
    /// The program runs as invoker (no auto-elevation); if not admin,
    /// it prompts the user to re-run as administrator and exits.
    /// </summary>
    public static class AdminCheck
    {
        public static void EnsureAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    Console.WriteLine();
                    Console.WriteLine(Loc.Get("Error_NeedElevate"));
                    Console.WriteLine();
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Loc.Format("Error_Unexpected", ex.Message));
                Environment.Exit(1);
            }
        }
    }
}
