using System;

namespace WTGUtility.Infrastructure
{
    /// <summary>
    /// Centralized console output helpers with color support.
    /// </summary>
    public static class ConsoleOutput
    {
        public static void WriteLine(string message = "")
        {
            Console.WriteLine(message);
        }

        public static void WriteWarning(string message)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }

        public static void WriteError(string message)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }

        public static void WriteBanner(string line1, string line2)
        {
            Console.WriteLine();
            Console.WriteLine(line1);
            Console.WriteLine(line2);
        }

        public static void WriteSeparator()
        {
            Console.WriteLine();
        }
    }
}
