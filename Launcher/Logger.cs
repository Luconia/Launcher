using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public static void LogException(Exception? exception)
        {
            var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = $@"{roamingDirectory}\Luconia\logs\{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss_tt", new CultureInfo("en-US"))}.txt";

            if (!Directory.Exists(roamingDirectory + "\\Luconia"))
            {
                Logger.LogWarning("Can't find Luconia directory");
                Logger.LogInfo("Creating Directory...");

                Directory.CreateDirectory(roamingDirectory + "\\Luconia");
                Logger.LogInfo("Directory created");
            }

            if (!Directory.Exists(roamingDirectory + "\\Luconia\\logs"))
            {
                Logger.LogWarning("Can't find logs directory");
                Logger.LogInfo("Creating logs directory...");

                Directory.CreateDirectory(roamingDirectory + "\\Luconia\\logs");
                Logger.LogInfo("logs directory created");
            }

            if (File.Exists(filePath)) File.Create(filePath).Close();
            File.WriteAllText(filePath, exception?.ToString());

            MessageBox.Show(exception?.ToString(), "An error has occured!", MessageBoxButton.OK, MessageBoxImage.Error);
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
