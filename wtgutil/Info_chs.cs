using System;

namespace WTG_Utility.Info_CHS
{
    internal class Information
    {
        internal static void GetHelp()
        {
            Console.WriteLine("用法: " + Environment.NewLine +
                              "  wtgutil.exe [/命令] [-参数]" + Environment.NewLine);
            Console.WriteLine("   /mode -- 选择系统设置" + Environment.NewLine +
                              "     -wintogo -- 使用 WinToGo 设置." + Environment.NewLine +
                              "     -default -- 使用系统默认设置." + Environment.NewLine);
            Console.WriteLine("   /partmgr -- 修改本地磁盘显示设置" + Environment.NewLine +
                              "     -showlocaldisks -- 系统启动时显示本地磁盘." + Environment.NewLine +
                              "     -hidelocaldisks -- 系统启动时隐藏本地磁盘." + Environment.NewLine);
            Console.WriteLine("   /uasp -- 修改系统UASP设置" + Environment.NewLine +
                              "     -disable -- 禁用UASP以实现“拔出冻结”，避免WTG设备意外断开连接导致系统崩溃." + Environment.NewLine +
                              "     --disable-force -- 修改系统设置强行禁用UASP，这可能导致意外情况." + Environment.NewLine +
                              "     --disable-force-restore -- 取消强行禁用UASP (如果WinToGo仍能启动)" + Environment.NewLine);
            Console.WriteLine("   /info -- 显示系统设置信息.");
            Console.WriteLine("   /about -- 关于本程序.");
            Console.WriteLine("   /help, /? -- 显示帮助信息." + Environment.NewLine);
            Console.WriteLine("   示例：" + Environment.NewLine +
                              "     wtgutil.exe /?" + Environment.NewLine +
                              "     wtgutil.exe /mode -wintogo" + Environment.NewLine +
                              "     wtgutil.exe /partmgr -hidelocaldisks");
        }
        internal static void GetAbout()
        {
            Console.WriteLine("WinToGo 实用程序 v1.12.3" + Environment.NewLine + "by Charles." + Environment.NewLine);
            Console.WriteLine("Github 存储库: https://github.com/Nothing9495/wtgutil" + Environment.NewLine);
            Console.WriteLine("上一次构建时间: 2025/04/03" + Environment.NewLine);
            Console.WriteLine("WinToGo 实用程序 (wtgutil) 是一个自由、开源的软件");
            Console.WriteLine("如果你在使用本工具的过程中遇到任何问题，" + Environment.NewLine +
                              "或者有任何想法和建议，欢迎到 Github 上提交 issues.");
        }
    }

    internal class Message
    {
        internal static void ShowWelcomeMsg()
        {
            Console.WriteLine("WindowsToGo 实用程序");
            Console.WriteLine("程序版本: v1.12.3");
        }
        internal static void ShowCompletedMsg()
        {
            Console.WriteLine("操作成功完成" + Environment.NewLine);
        }
        internal static void ShowRestartMsg()
        {
            Console.WriteLine("重启使部分更改生效" + Environment.NewLine);
        }
        internal static void ShowWarningMsg()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("警告: 执行此操作后，你的 WinToGo 工作站将不再支持从 USB 设备启动，" + Environment.NewLine +
                              "      因此请不要轻易地重启你的 WinToGo 工作站." + Environment.NewLine);
            Console.ResetColor();
        }
        internal static void ShowUnknownArgMsg()
        {
            Console.WriteLine();
            Console.WriteLine("未知命令.");
            Console.WriteLine("使用 \"/help\" 或 \"/?\" 来获取帮助.");
            Console.WriteLine();
        }
        internal static void ShowUnknownParamMsg()
        {
            Console.WriteLine();
            Console.WriteLine("未知参数.");
            Console.WriteLine("使用 \"/help\" 或 \"/?\" 来获取帮助.");
            Console.WriteLine();
        }
        internal static void ShowNoValidParamMsg()
        {
            Console.WriteLine();
            Console.WriteLine("未接受到有效参数.");
            Console.WriteLine("使用 \"/help\" 或 \"/?\" 来获取帮助.");
            Console.WriteLine();
        }
        internal static void ShowWarningMsg_FUASP()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("警告: 这个操作可能导致系统损坏，甚至在下次启动时出现崩溃" + Environment.NewLine +
                              "      如果你在应用后发生了崩溃，请尝试在其他电脑上还原以下注册表设置." + Environment.NewLine +
                                                                                                                                            Environment.NewLine +
                              "             注册表路径:  HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Services\\UASPSTOR" + Environment.NewLine +
                              "             注册表键名:  ImagePath, Owners" + Environment.NewLine +
                              "             原始值:      \\SystemRoot\\System32\\drivers\\uaspstor.sys, uaspstor.inf" + Environment.NewLine);
            Console.WriteLine("继续代表你了解相关风险，并且有能力修复他们.");
            Console.ResetColor();
            Console.WriteLine();
        }
        internal static void ShowWaitingMsg()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("按 \"Y\" 以继续, 或者按 \"N\" 取消.");
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
                Console.WriteLine("操作已被用户取消.");
                Console.WriteLine();
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("无效输入.");
                ShowWaitingMsg();
            }
        }
        internal static void CurrentInfoText()
        {
            Console.WriteLine("当前系统信息:");
        }
    }
}
