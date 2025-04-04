using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using WTG_Utility.Functions; //To change language, append "_CHS". like "Functions_CHS".
using WTG_Utility.Info; //To change language, append "_CHS". like "Info_CHS".

namespace WTG_Utility
{
    class Program
    {
        static void Main(string[] args) //a bunch of if-else
        {

            Console.Title = ("wtgutil");

            IsAdmin.IsAdministrator();

            string deviceInstancePath = GetSettings.GetWTGDriveInstancePath();
            if (deviceInstancePath == null)
            {
                Console.WriteLine();
                Console.WriteLine("An error occurred: No WindowsToGo drive detected.");
            }

            if (args.Count() <= 0)
            {
                Console.WriteLine();
                Information.GetHelp();
                Console.WriteLine();
            }
            else if (args.Count() == 1)
            {
                string argToLower = args[0];
                string arg = argToLower.ToLower();
                if (arg == "/info")
                {

                    Console.WriteLine();
                    Message.ShowWelcomeMsg();
                    Console.WriteLine();
                    Message.CurrentInfoText();
                    GetSettings.GetBootDriverFlags();
                    GetSettings.GetPortableOSFeature();
                    GetSettings.GetPartmgrSettings();
                    GetSettings.GetUASPStatus(deviceInstancePath);
                    Console.WriteLine();
                    Message.ShowCompletedMsg();

                }
                else if (arg == "/help" || arg == "/?")
                {

                    Console.WriteLine();
                    Message.ShowWelcomeMsg();
                    Console.WriteLine();
                    Information.GetHelp();
                    Console.WriteLine();
                    Message.ShowCompletedMsg();

                }
                else if (arg == "/about")
                {

                    Console.WriteLine();
                    Information.GetAbout();
                    Console.WriteLine();
                    Message.ShowCompletedMsg();

                }
                else if (arg == "/mode")
                {

                    Message.ShowNoValidParamMsg();

                }
                else if (arg == "/partmgr")
                {

                    Message.ShowNoValidParamMsg();

                }
                else if (arg == "/uasp")
                {
                    Message.ShowNoValidParamMsg();
                }
                else
                {

                    Message.ShowUnknownArgMsg();

                }
            }
            else if (args.Count() == 2)
            {
                string argToLower = args[0];
                string arg = argToLower.ToLower();
                string paramToLower = args[1];
                string param = paramToLower.ToLower();

                if (arg == "/mode")
                {
                    if (param == "-wintogo")
                    {

                        Console.WriteLine();
                        Message.ShowWelcomeMsg();
                        Console.WriteLine();
                        ModifySettings.SetBootDriverFlags(20);
                        ModifySettings.SetPortableOSFeature(1);
                        ModifySettings.SetPartmgrSettings(4);
                        Message.ShowRestartMsg();
                        Message.ShowCompletedMsg();

                    }
                    else if (param == "-default")
                    {

                        Console.WriteLine();
                        Message.ShowWelcomeMsg();
                        Console.WriteLine();
                        ModifySettings.SetBootDriverFlags(0);
                        ModifySettings.SetPortableOSFeature(0);
                        ModifySettings.SetPartmgrSettings(1);
                        Message.ShowRestartMsg();
                        Message.ShowCompletedMsg();
                        Message.ShowWarningMsg();

                    }
                    else
                    {

                        Message.ShowUnknownParamMsg();

                    }
                }
                else if (arg == "/partmgr")
                {
                    if (param == "-showlocaldisks")
                    {

                        Console.WriteLine();
                        Message.ShowWelcomeMsg();
                        Console.WriteLine();
                        ModifySettings.SetPartmgrSettings(1);
                        Message.ShowRestartMsg();
                        Message.ShowCompletedMsg();

                    }
                    else if (param == "-hidelocaldisks")
                    {

                        Console.WriteLine();
                        Message.ShowWelcomeMsg();
                        Console.WriteLine();
                        ModifySettings.SetPartmgrSettings(4);
                        Message.ShowRestartMsg();
                        Message.ShowCompletedMsg();

                    }
                    else
                    {

                        Message.ShowUnknownParamMsg();

                    }
                }
                else if (arg == "/uasp")
                {
                    if (param == "-disable")
                    {
                        Console.WriteLine();
                        Message.ShowWelcomeMsg();
                        ModifySettings.DisableUASP(deviceInstancePath);
                        Console.WriteLine();
                        Message.ShowCompletedMsg();
                    }
                    else if (param == "--disable-force")
                    {
                        Console.WriteLine();
                        Message.ShowWelcomeMsg();
                        Console.WriteLine();
                        Message.ShowWarningMsg_FUASP();
                        Message.ShowWaitingMsg();
                        ModifySettings.DisableUASPbyReplaceDriverConfig();
                        Console.WriteLine();
                        Message.ShowCompletedMsg();
                    }
                    else if (param == "--disable-force-restore")
                    {
                        Console.WriteLine();
                        Message.ShowWelcomeMsg();
                        ModifySettings.RestoreUASPDriverConfig();
                        Console.WriteLine();
                        Message.ShowCompletedMsg();
                    }
                    else
                    {
                        Message.ShowUnknownParamMsg();
                    }
                }
                else
                {

                    Message.ShowUnknownArgMsg();

                }
            }
        }
    }
}
