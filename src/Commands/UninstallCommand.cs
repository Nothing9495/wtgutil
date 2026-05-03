using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class UninstallCommand : ICommand
    {
        public string Name => "uninstall";
        public string Description => Loc.Get("Cmd_Uninstall_Desc");
        public string SubCommandHelp => string.Empty;

        public Task<int> ExecuteAsync(string[] args)
        {
            return Task.FromResult(0);
        }

        public static int Execute(CommandContext ctx)
        {
            ConsoleOutput.WriteBanner(Loc.Get("App_Title"), Loc.Format("App_Version", Loc.Version));
            ConsoleOutput.WriteSeparator();

            try
            {
                // Guard: refuse if not properly installed via 'wtgutil install'
                if (!IsInstalled())
                {
                    ConsoleOutput.WriteSeparator();
                    ConsoleOutput.WriteError(Loc.Get("Error_Uninstall_NotInstalled"));
                    return 1;
                }

                string programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
                string installDir = Path.Combine(programFiles, "wtgutil");
                string destPath = Path.Combine(installDir, "wtgutil.exe");
                string aliasPath = Path.Combine(installDir, "wtgu.exe");

                bool removed = false;
                string currentPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                bool runningFromInstallDir = currentPath.Equals(destPath, StringComparison.OrdinalIgnoreCase);

                // Remove wtgu.exe hard link alias
                if (File.Exists(aliasPath))
                {
                    File.Delete(aliasPath);
                    removed = true;
                }

                // Remove wtgutil.exe
                if (File.Exists(destPath))
                {
                    if (runningFromInstallDir)
                    {
                        // Can't delete a running executable — schedule delayed deletion
                        ScheduleSelfDeletion(installDir);
                    }
                    else
                    {
                        File.Delete(destPath);
                    }
                    removed = true;
                }

                // Remove install directory from system PATH
                RemoveFromSystemPath(installDir);

                // Clean up empty directory
                if (Directory.Exists(installDir))
                {
                    try
                    {
                        Directory.Delete(installDir, recursive: false);
                    }
                    catch (IOException) { /* directory not empty, leave it */ }
                }

                // Notify the system about environment change
                NotifyEnvironmentChange();

                // Finish uninstallation
                if (removed)
                {
                    // Clear the install flag
                    ClearInstallFlag();

                    ConsoleOutput.WriteLine(Loc.Format("Msg_Uninstall_Success",
                        "https://github.com/Nothing9495/wtgutil"));
                    ConsoleOutput.WriteSeparator();
                    Console.WriteLine(Loc.Get("Msg_Completed"));
                }
                else
                {
                    ConsoleOutput.WriteWarning(Loc.Get("Error_Uninstall_NotFound"));
                }

                return 0;
            }
            catch (Exception ex)
            {
                ConsoleOutput.WriteError(Loc.Format("Error_Unexpected", ex.Message));
                return 1;
            }
        }

        /// <summary>
        /// Checks whether wtgutil was installed via 'wtgutil install'.
        /// </summary>
        private static bool IsInstalled()
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"Software\wtgutil");
            if (key == null) return false;

            object value = key.GetValue("InstallFlag", 0);
            return value is int flag && flag == 1;
        }

        /// <summary>
        /// Removes the install flag from HKLM\Software\wtgutil.
        /// </summary>
        private static void ClearInstallFlag()
        {
            try
            {
                Registry.LocalMachine.DeleteSubKeyTree(@"Software\wtgutil", throwOnMissingSubKey: false);
            }
            catch { /* key already gone */ }
        }

        /// <summary>
        /// Removes the wtgutil directory entry from the system PATH (HKLM).
        /// </summary>
        private static void RemoveFromSystemPath(string directory)
        {
            const string pathKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            using (var key = Registry.LocalMachine.OpenSubKey(pathKey, writable: true))
            {
                if (key == null) return;

                string currentPath = (string)key.GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);
                string pathEntry = @"%ProgramFiles%\wtgutil";

                if (currentPath.IndexOf(pathEntry, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string[] parts = currentPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    var filtered = new System.Collections.Generic.List<string>();
                    foreach (var part in parts)
                    {
                        if (!part.Equals(pathEntry, StringComparison.OrdinalIgnoreCase))
                            filtered.Add(part);
                    }
                    string newPath = string.Join(";", filtered);
                    key.SetValue("Path", newPath, RegistryValueKind.ExpandString);
                }
            }
        }

        /// <summary>
        /// Broadcasts WM_SETTINGCHANGE so running processes pick up the new PATH.
        /// </summary>
        private static void NotifyEnvironmentChange()
        {
            const int HWND_BROADCAST = 0xffff;
            const int WM_SETTINGCHANGE = 0x001a;
            const int SMTO_ABORTIFHUNG = 0x0002;

            SendMessageTimeout(
                (IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE,
                IntPtr.Zero, "Environment",
                SMTO_ABORTIFHUNG, 5000, out _);
        }

        /// <summary>
        /// Spawns a PowerShell process that watches the current PID and deletes
        /// the specified file once this process has fully exited.
        /// </summary>
        private static void ScheduleSelfDeletion(string filePath)
        {
            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"$p = Get-Process -Id {pid} -ErrorAction SilentlyContinue; if ($p) {{ $p.WaitForExit() }}; Remove-Item -Force '{filePath}' -Recurse\"",
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };
            System.Diagnostics.Process.Start(psi);
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd, uint Msg, IntPtr wParam, string lParam,
            uint fuFlags, uint uTimeout, out IntPtr lpdwResult);
    }
}
