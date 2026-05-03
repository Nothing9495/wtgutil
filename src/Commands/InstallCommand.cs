using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class InstallCommand : ICommand
    {
        public string Name => "install";
        public string Description => Loc.Get("Cmd_Install_Desc");
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
                string sourcePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                string programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
                string installDir = Path.Combine(programFiles, "wtgutil");
                string destPath = Path.Combine(installDir, "wtgutil.exe");
                string aliasPath = Path.Combine(installDir, "wtgu.exe");

                // Create installation directory
                Directory.CreateDirectory(installDir);

                // Copy self to %ProgramFiles%\wtgutil\
                File.Copy(sourcePath, destPath, overwrite: true);

                // Create hard link alias: wtgu.exe -> wtgutil.exe
                RunMklink(aliasPath, destPath);

                // Add install directory to system PATH
                AddToSystemPath(installDir);

                // Notify the system about environment change
                NotifyEnvironmentChange();

                // Verify the installation
                if (File.Exists(destPath))
                {
                    // Write install flag to registry
                    SetInstallFlag(1);

                    ConsoleOutput.WriteLine(Loc.Get("Msg_Install_Success"));
                    ConsoleOutput.WriteSeparator();
                    Console.WriteLine(Loc.Get("Msg_Completed"));

                    // Schedule self-deletion of the original executable
                    ScheduleSelfDeletion(sourcePath);

                    return 0;
                }
                else
                {
                    ConsoleOutput.WriteError(Loc.Get("Error_Install_Failed"));
                    return 1;
                }
            }
            catch (Exception ex)
            {
                ConsoleOutput.WriteError(Loc.Format("Error_Unexpected", ex.Message));
                return 1;
            }
        }

        /// <summary>
        /// Creates a hard link using mklink /h via cmd.exe.
        /// </summary>
        private static void RunMklink(string linkPath, string targetPath)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c mklink /h \"{linkPath}\" \"{targetPath}\"",
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };
            using var process = System.Diagnostics.Process.Start(psi);
            process?.WaitForExit();
        }

        /// <summary>
        /// Adds a directory to the system PATH (HKLM) if not already present.
        /// </summary>
        private static void AddToSystemPath(string directory)
        {
            const string pathKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            using (var key = Registry.LocalMachine.OpenSubKey(pathKey, writable: true))
            {
                if (key == null) return;

                string currentPath = (string)key.GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);
                string pathEntry = @"%ProgramFiles%\wtgutil";

                if (currentPath.IndexOf(pathEntry, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    string newPath = currentPath.TrimEnd(';') + ";" + pathEntry;
                    key.SetValue("Path", newPath, RegistryValueKind.ExpandString);
                }
            }
        }

        /// <summary>
        /// Sets or removes the install flag at HKLM\Software\wtgutil\InstallFlag.
        /// </summary>
        private static void SetInstallFlag(int value)
        {
            using var key = Registry.LocalMachine.CreateSubKey(@"Software\wtgutil");
            key.SetValue("InstallFlag", value, RegistryValueKind.DWord);
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

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd, uint Msg, IntPtr wParam, string lParam,
            uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        /// <summary>
        /// Creates a background batch script that watches the current PID and deletes
        /// the specified file once this process has fully exited.
        /// </summary>
        private static void ScheduleSelfDeletion(string filePath)
        {
            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"$p = Get-Process -Id {pid} -ErrorAction SilentlyContinue; if ($p) {{ $p.WaitForExit() }}; Remove-Item -Force '{filePath}'\"",
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };
            System.Diagnostics.Process.Start(psi);
        }
    }
}
