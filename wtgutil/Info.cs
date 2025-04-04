using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace WTG_Utility.Info
{
    internal class Information
    {

        internal static void GetHelp()
        {
            Console.WriteLine("Usage: " + Environment.NewLine +
                              "  wtgutil.exe [/commmand] [-paramter]" + Environment.NewLine);
            Console.WriteLine("   /mode -- Choose system settings." + Environment.NewLine +
                              "     -wintogo -- Use WinToGo settings." + Environment.NewLine +
                              "     -default -- Use system default settings." + Environment.NewLine);
            Console.WriteLine("   /partmgr -- Modify local disks display settings" + Environment.NewLine +
                              "     -showlocaldisks -- Show local disks when the system startup." + Environment.NewLine +
                              "     -hidelocaldisks -- Hide local disks when the system startup." + Environment.NewLine);
            Console.WriteLine("   /uasp -- Modify UASP Settings" + Environment.NewLine +
                              "     -disable -- Disable UASP in WinToGo to enable system freeze when system drive unplugged." + Environment.NewLine +
                              "     --disable-force -- Disable UASP by modifying driver config. This may cause damage and lead to tricky situation." + Environment.NewLine +
                              "     --disable-force-restore -- Restore changes made by --disable-force (if your system doesn't run into crash)" + Environment.NewLine);
            Console.WriteLine("   /info -- Show system settings status.");
            Console.WriteLine("   /about -- About this utility.");
            Console.WriteLine("   /help, /? -- Show help information." + Environment.NewLine);
            Console.WriteLine("   Examples：" + Environment.NewLine +
                              "     wtgutil.exe /?" + Environment.NewLine +
                              "     wtgutil.exe /mode -wintogo" + Environment.NewLine +
                              "     wtgutil.exe /partmgr -hidelocaldisks");
        }
        internal static void GetAbout()
        {
            Console.WriteLine("WindowsToGo Utility v1.12.3" + Environment.NewLine + "by charlesy" + Environment.NewLine);
            Console.WriteLine("Github repository: https://github.com/Nothing9495/wtgutil" + Environment.NewLine);
            Console.WriteLine("Last build date: 03/04/2025" + Environment.NewLine);
            Console.WriteLine("wtgutil (WinToGo Utility) is a free and open-source program");
            Console.WriteLine("If you come across any problems when you are using this utility," + Environment.NewLine + 
                              "or you have any suggestions, it is welcomed to submit them on Github issues.");
        }
    }

    internal class Message
    {
        internal static void ShowWelcomeMsg()
        {
            Console.WriteLine("WindowsToGo Utility");
            Console.WriteLine("Version: v1.12.3");
        }
        internal static void ShowCompletedMsg()
        {
            Console.WriteLine("The operation completed successfully." + Environment.NewLine);
        }
        internal static void ShowRestartMsg()
        {
            Console.WriteLine("Restart to make some of the changes take effect." + Environment.NewLine);
        }
        internal static void ShowWarningMsg()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning: Do not reboot your WinToGo workstation after executing this operation," + Environment.NewLine +
                              "         since your workstation no longer supports booting from USB devices." + Environment.NewLine);
            Console.ResetColor();
        }
        internal static void ShowUnknownArgMsg()
        {
            Console.WriteLine();
            Console.WriteLine("Unknown command.");
            Console.WriteLine("Type \"/help\" or \"/?\" for help.");
            Console.WriteLine();
        }
        internal static void ShowUnknownParamMsg()
        {
            Console.WriteLine();
            Console.WriteLine("Unknown paramter.");
            Console.WriteLine("Type \"/help\" or \"/?\" for help.");
            Console.WriteLine();
        }
        internal static void ShowNoValidParamMsg()
        {
            Console.WriteLine();
            Console.WriteLine("No valid paramter.");
            Console.WriteLine("Type \"/help\" or \"/?\" for help.");
            Console.WriteLine();
        }
        internal static void ShowWarningMsg_FUASP()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Critical warning: This operation may cause damage to your system, and leading to BSOD the next time it boots." + Environment.NewLine +
                              "                  If BSOD happend after applying it, try to reset the specific registry key in another PC." + Environment.NewLine +
                                                                                                                                            Environment.NewLine +
                              "                  Registry Path to Modify: HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Services\\UASPSTOR" + Environment.NewLine + 
                              "                  Registry Key to Modify:  ImagePath, Owners"                                             + Environment.NewLine +
                              "                  Original Key Value:      \\SystemRoot\\System32\\drivers\\uaspstor.sys, uaspstor.inf" + Environment.NewLine);
            Console.WriteLine("Continue means you've learned the potential risks and you are capable of dealing with them.");
            Console.ResetColor();
            Console.WriteLine();
        }
        internal static void ShowWaitingMsg()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press \"Y\" to contiune, or \"N\" to cancel.");
            Console.ResetColor();
            char C = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (C == 'Y' || C == 'y')
            {
                // Continue
            }
            else if (C == 'N' || C == 'n')
            {
                Console.WriteLine();
                Console.WriteLine("Operation has been cancelled by user.");
                Console.WriteLine();
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input.");
                ShowWaitingMsg();
            }
        }
        internal static void CurrentInfoText()
        {
            Console.WriteLine("Current Info:");
        }
    }
}
