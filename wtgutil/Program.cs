using System;
using System.Linq;
using WTG_Utility.Functions; //To change language, add "_CHS" to the end of it. Like "Functions_CHS".
using WTG_Utility.Info;      //To change language, add "_CHS" to the end of it. Like "Info_CHS".

namespace WTG_Utility
{
    class Program
    {
        static void Main(string[] args) //a bunch of if-else
        {

            Console.Title = ("WTG Util v3");

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
                    GetSettings.CurrentInfo();
                    GetSettings.GetBootDriverFlags();
                    GetSettings.GetPortableOSFeature();
                    GetSettings.GetPartmgrSettings();
                    Console.WriteLine();
                    Message.ShowCompletedMsg();

                }
                else if (arg == "/help" || arg == "/?")
                {

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

                        ModifySettings.SetBootDriverFlags(20);
                        ModifySettings.SetPortableOSFeature(1);
                        ModifySettings.SetPartmgrSettings(4);
                        Console.WriteLine();
                        Message.ShowRestartMsg();
                        Message.ShowCompletedMsg();

                    }
                    else if (param == "-default")
                    {

                        ModifySettings.SetBootDriverFlags(0);
                        ModifySettings.SetPortableOSFeature(0);
                        ModifySettings.SetPartmgrSettings(1);
                        Console.WriteLine();
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

                        ModifySettings.SetPartmgrSettings(1);
                        Console.WriteLine();
                        Message.ShowRestartMsg();
                        Message.ShowCompletedMsg();

                    }
                    else if (param == "-hidelocaldisks")
                    {

                        ModifySettings.SetPartmgrSettings(4);
                        Console.WriteLine();
                        Message.ShowRestartMsg();
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