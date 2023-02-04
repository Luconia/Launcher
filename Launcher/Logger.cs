using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    internal class Logger
    {
        public static void LogInfo(string message)
        {
            Log(ConsoleColor.DarkCyan, "[Info]", ConsoleColor.Cyan, message);
        }

        public static void LogInfo(string format, object? arg0)
        {
            Log(ConsoleColor.DarkCyan, "[Info]", ConsoleColor.Cyan, format, arg0);
        }

        public static void LogWarning(string message)
        {
            Log(ConsoleColor.DarkYellow, "[Warning]", ConsoleColor.Yellow, message);
        }

        public static void LogWarning(string format, object? arg0)
        {
            Log(ConsoleColor.DarkYellow, "[Warning]", ConsoleColor.Yellow, format, arg0);
        }

        public static void LogError(string message)
        {
            Log(ConsoleColor.DarkRed, "[Error]", ConsoleColor.Red, message);
        }

        public static void LogError(string format, object? arg0)
        {
            Log(ConsoleColor.DarkRed, "[Error]", ConsoleColor.Red, format, arg0);
        }

        public static void Log(ConsoleColor prefixColor, string prefix, ConsoleColor messageColor, object? message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[" + DateTime.Now.ToString("hh:mm:ss tt", new CultureInfo("en-US")) + "] ");

            Console.ForegroundColor = prefixColor;
            Console.Write("{0} ", prefix);

            Console.ForegroundColor = messageColor;
            Console.WriteLine(message);

            Console.ResetColor();
        }
        public static void Log(ConsoleColor prefixColor, string prefix, ConsoleColor messageColor, string format, object? arg0)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[" + DateTime.Now.ToString("hh:mm:ss tt", new CultureInfo("en-US")) + "] ");

            Console.ForegroundColor = prefixColor;
            Console.Write("{0} ", prefix);

            Console.ForegroundColor = messageColor;
            Console.WriteLine(format, arg0);

            Console.ResetColor();
        }
    }
}
