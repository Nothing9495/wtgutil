using System;

namespace WTG_Utility.Info
{
    internal class Information
    {
        internal static void GetHelp()
        {
            Console.WriteLine("Usage: " + Environment.NewLine +
                              "  wtgutil [/arguments] [-paramters]" + Environment.NewLine);
            Console.WriteLine("   /mode -- Choose system settings." + Environment.NewLine +
                              "     -wintogo -- Use WinToGo settings." + Environment.NewLine +
                              "     -default -- Use system default settings." + Environment.NewLine);
            Console.WriteLine("   /partmgr -- Modify local disks display settings" + Environment.NewLine +
                              "     -showlocaldisks -- Show local disks when the system startup." + Environment.NewLine +
                              "     -hidelocaldisks -- Hide local disks when the system startup." + Environment.NewLine);
            Console.WriteLine("   /info -- Show system settings status.");
            Console.WriteLine("   /about -- About this utility.");
            Console.WriteLine("   /help, /? -- Show help information.");
        }
        internal static void GetAbout()
        {
            Console.WriteLine("WinToGo Utility v3" + Environment.NewLine + "by Charles." + Environment.NewLine);
            Console.WriteLine("Github repository: https://github.com/Nothing9495/wtgutil" + Environment.NewLine);
            Console.WriteLine("Last build: 11/20/2022" + Environment.NewLine);
            Console.WriteLine("If you come across any problems when you are using this utility," + Environment.NewLine + 
                              "or you have any suggestions, it is welcomed to submit them to Github issues.");
        }
    }

    internal class Message
    {
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
            Console.WriteLine("Unknown argument.");
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
    }
}
