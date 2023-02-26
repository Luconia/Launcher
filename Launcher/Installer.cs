﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Net;
using System.Windows.Controls;
using DiscordRPC;

namespace Launcher
{
    internal class Installer
    {
        static string roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string version = "";

        public Installer() 
        {
            if (!Directory.Exists(roamingDirectory + "\\Luconia"))
            {
                Logger.LogWarning("Can't find Luconia directory");
                Logger.LogInfo("Creating Directory...");

                Directory.CreateDirectory(roamingDirectory + "\\Luconia");
                Logger.LogInfo("Directory created");
            }

            if (!File.Exists(roamingDirectory + "\\Luconia\\type.txt"))
            {
                File.Create(roamingDirectory + "\\Luconia\\type.txt").Close();
                File.WriteAllText(roamingDirectory + "\\Luconia\\type.txt", "normal\n// set to debug if you want to see the console");
            }

            if (File.ReadLines(roamingDirectory + "\\Luconia\\type.txt").First() == "debug")
            {
                MainWindow.AllocConsole();
            }

            Logger.LogInfo("Initializing Installer");
            InitVersion();
            InitDiscordRichPresence();
        }

        private void InitDiscordRichPresence()
        {
            if (!CheckNet()) return;

            DiscordRpcClient client = new DiscordRpcClient("1066805525540507728");

            client.OnReady += (sender, e) =>
            {
                Logger.LogInfo("Received Ready from user {0}", e.User.Username);
            };

            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Details = $"Luconia v{version}",
                //State = "In the Launcher",
                Assets = new Assets()
                {
                    LargeImageKey = "icon",
                    LargeImageText = $"v{version}",
                },
                Buttons = new DiscordRPC.Button[]
                {
                    new DiscordRPC.Button()
                    {
                        Label = "Join the Discord", Url = "https://discord.gg/luconia"
                    }
                }
            });
        }

        private async void InitVersion()
        {
            var existsFile = File.Exists(roamingDirectory + "\\Luconia\\version.txt");

            if (!CheckNet()) return;

            if (!existsFile) 
            {
                Logger.LogWarning("Can't find version file");
                Logger.LogInfo("Creating File...");
                File.Create(roamingDirectory + "\\Luconia\\version.txt").Close();
                File.WriteAllText(roamingDirectory + "\\Luconia\\version.txt", null);
                Logger.LogInfo("File created");
                if (Utils.CheckNet()) File.WriteAllText(roamingDirectory + "\\Luconia\\version.txt", await new HttpClient().GetStringAsync("https://media.luconia.net/version.txt"));
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

            if (!CheckNet()) return;

            var existsFile = File.Exists(roamingDirectory + "\\Luconia\\luconia.dll");
            var latestVersion = await new HttpClient().GetStringAsync("https://media.luconia.net/version.txt");

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
                    wc.DownloadFileAsync(new Uri($"https://media.luconia.net/luconia/{latestVersion}/luconia.dll"), roamingDirectory + "\\Luconia\\luconia.dll");

                    window.launchButton.Content = "Downloading...";
                    window.launchButton.IsEnabled = false;

                    File.WriteAllText(roamingDirectory + "\\Luconia\\version.txt", latestVersion);
                }
            }
        }

        private static bool CheckNet()
        {
            if (!Utils.CheckNet())
            {
                Logger.LogError("No internet found!");
                MessageBox.Show("You need internet to use the launcher!", "An error has occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.MainWindow.Close();
                return false;
            }
            return true;
        }
    }
}
