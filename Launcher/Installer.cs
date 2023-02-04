using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Net;
using System.Windows.Controls;

namespace Launcher
{
    internal class Installer
    {
        static string roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string version = "";

        public Installer() 
        {
            Logger.LogInfo("Initializing Installer");
            InitVersion();
        }

        private async void InitVersion()
        {
            var existsFile = File.Exists(roamingDirectory + "\\Luconia\\version.txt");
            var existsDirectory = Directory.Exists(roamingDirectory + "\\Luconia");

            if (!existsDirectory)
            {
                Logger.LogWarning("Can't find Luconia directory");
                Logger.LogInfo("Creating Directory...");

                Directory.CreateDirectory(roamingDirectory + "\\Luconia");
                Logger.LogInfo("Directory created");
            }

            if (!Utils.CheckNet())
            {
                Logger.LogError("No internet found!");
                MessageBox.Show("You need internet to use the launcher!", "An error has occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.MainWindow.Close();
                return;
            }

            if (!existsFile) 
            {
                Logger.LogWarning("Can't find version file");
                Logger.LogInfo("Creating File...");
                File.Create(roamingDirectory + "\\Luconia\\version.txt").Close();
                Logger.LogInfo("File created");
                File.WriteAllText(roamingDirectory + "\\Luconia\\version.txt", await new HttpClient().GetStringAsync("https://luconia.net/luconia/version.txt"));
            }

            version = File.ReadLines(roamingDirectory + "\\Luconia\\version.txt").First();
        }

        private static void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            MainWindow window = ((MainWindow)Application.Current.MainWindow);
            window.launchButton.Content = $"Downloading... {e.ProgressPercentage}%";
            window.launchButton.IsEnabled = false;
        }

        private static void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MainWindow window = ((MainWindow)Application.Current.MainWindow);

            if (e.Cancelled)
            {
                Logger.LogError("Download canceled!");
                return;
            }

            if (e.Error != null)
            {
                Logger.LogError($"Somethings wrong, check your internet!");
                Logger.LogError($"Error: {e.Error.Message}");
                return;
            }

            Logger.LogInfo("Finished downloading file!");
            
            window.launchButton.Content = "Launch";
            window.launchButton.IsEnabled = true;
        }

        public static async void Download()
        {
            MainWindow window = ((MainWindow)Application.Current.MainWindow);

            if (!Utils.CheckNet())
            {
                Logger.LogError("No internet found!");
                MessageBox.Show("You need internet to use the launcher!", "An error has occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.MainWindow.Close();
                return;
            }

            var existsFile = File.Exists(roamingDirectory + "\\Luconia\\luconia.dll");
            var latestVersion = await new HttpClient().GetStringAsync("https://luconia.net/luconia/version.txt");

            if (!existsFile || version != latestVersion)
            {
                if (version != latestVersion && existsFile)
                {
                    Logger.LogWarning("Update found!");
                    Logger.LogWarning($"-> v{latestVersion} current version: v{version}");
                }

                Logger.LogInfo("Downloading...");
                using (WebClient wc = new WebClient()) 
                {
                    wc.DownloadProgressChanged += DownloadProgressChanged;
                    wc.DownloadFileCompleted += DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri($"https://luconia.net/luconia/{latestVersion}/luconia.dll"), roamingDirectory + "\\Luconia\\luconia.dll");

                    window.launchButton.Content = "Downloading...";
                    window.launchButton.IsEnabled = false;

                    File.WriteAllText(roamingDirectory + "\\Luconia\\version.txt", latestVersion);
                }
            }
        }
    }
}
