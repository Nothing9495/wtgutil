using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.Win32;

namespace WTG_Switcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Apperance
            string Time = DateTime.Now.ToString("HH:mm:ss");
            Console.Title = ("WTG Switcher v2 " + "(Start Time: " + Time + ")");
            Console.WriteLine();
            Console.WriteLine("WTG Switcher Release v2");
            Console.WriteLine("Copyright (C) Charles.");
            //Check Status
            ////Get GUID
            RegistryKey BDF = Registry.LocalMachine.OpenSubKey("SYSTEM\\HardwareConfig");
            object BR = BDF.GetValue("BootDriverFlags");
            RegistryKey POS = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
            object PR = POS.GetValue("PortableOperatingSystem");
            ////Output
            Console.WriteLine();
            Console.WriteLine("Currnet Status:" +
                Environment.NewLine + " Boot Driver Settings: " + BR + 
                Environment.NewLine + " PortableOS Feature: " + PR);
            Console.WriteLine();
            Console.WriteLine("     For Boot Driver Settings, 20 means USB mode, 0 means Local Disk mode" + 
                Environment.NewLine + "     For PortableOS Feature, 1 means enbale, 0 means disbaled.");


            //Menu
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("Which operation do you want to perform?");
            Console.WriteLine("1. Swtich to WTG Mode" +
               Environment.NewLine + "2. Switch to Normal Mode" +
               Environment.NewLine + "3. Exit");

            Console.WriteLine();
            var input = Console.ReadKey();
            var key = input.KeyChar;
            int value;

            if (int.TryParse(key.ToString(), out value))
            {
                Console.WriteLine();
                RouteChoice(value);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("\nInvalid Entry.");
                Process.GetCurrentProcess().Kill();
            }


        }

        private static void RouteChoice(int Choice)
        {
        //Switch
        string Key1 = "SYSTEM\\HardwareConfig";
        string BDF = "BootDriverFlags";
        string Key2 = "SYSTEM\\CurrentControlSet\\Control";
        string POS = "PortableOperatingSystem";

            switch (Choice)
            {
                //Switch On
                case 1:
                    RegistryKey BootDriverUSB = Registry.LocalMachine.CreateSubKey(Key1);
                    BootDriverUSB.SetValue(BDF, 20, RegistryValueKind.DWord);
                    BootDriverUSB.Close();
                    RegistryKey Portable = Registry.LocalMachine.CreateSubKey(Key2);
                    Portable.SetValue(POS, 1, RegistryValueKind.DWord);
                    Portable.Close();

                    Console.WriteLine();
                    Console.WriteLine("Done.");
                    Console.Write("Press any key to exit...");
                    Console.ReadKey(true);
                    Console.WriteLine();
                    Process.GetCurrentProcess().Kill();
                    break;

                //Switch Off
                case 2:
                    RegistryKey BootDriverLDisk = Registry.LocalMachine.CreateSubKey(Key1);
                    BootDriverLDisk.SetValue(BDF, 0, RegistryValueKind.DWord);
                    BootDriverLDisk.Close();
                    RegistryKey Normal = Registry.LocalMachine.CreateSubKey(Key2);
                    Normal.SetValue(POS, 0, RegistryValueKind.DWord);
                    Normal.Close();

                    Console.WriteLine();
                    Console.WriteLine("Done.");
                    Console.Write("Press any key to exit...");
                    Console.ReadKey(true);
                    Console.WriteLine();
                    Process.GetCurrentProcess().Kill();
                    break;

                //Exit
                default:
                    Console.WriteLine();
                    Console.WriteLine("Exit in 3s.");
                    Thread.Sleep(3000);
                    Console.WriteLine();
                    Process.GetCurrentProcess().Kill();
                    break;

            }
        }
    }
}
