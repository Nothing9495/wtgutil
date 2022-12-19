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
            Console.WriteLine("WinToGo 实用程序 v3.0.1" + Environment.NewLine + "by Charles." + Environment.NewLine);
            Console.WriteLine("Github 存储库: https://github.com/Nothing9495/wtgutil" + Environment.NewLine);
            Console.WriteLine("上一次构建时间: 2022/12/04" + Environment.NewLine);
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
            Console.WriteLine("程序版本: v3.0.1");
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
        internal static void CurrentInfoText()
        {
            Console.WriteLine("当前系统信息:");
        }
    }
}
