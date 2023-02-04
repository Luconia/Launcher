using DiscordRPC;
using DiscordRPC.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Launcher
{
    public partial class MainWindow : Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        Installer? installer;
        public static Process? Minecraft;

        bool isInjected = false;

        string roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists(roamingDirectory + "\\Luconia\\type.txt"))
            {
                File.Create(roamingDirectory + "\\Luconia\\type.txt").Close();
                File.WriteAllText(roamingDirectory + "\\Luconia\\type.txt", "normal\n// set to debug if you want to see the console");
            }

            if (File.ReadLines(roamingDirectory + "\\Luconia\\type.txt").First() == "debug")
            {
                AllocConsole();
            }

            if (!Utils.CheckNet())
            {
                Logger.LogError("No internet found!");
                MessageBox.Show("You need internet to use the launcher!", "An error has occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            installer = new Installer();

            Logger.LogInfo("Started Luconia Launcher");
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void CloseLauncher(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeLauncher(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // some things are from https://github.com/Plextora/LatiteInjector/blob/master/MainWindow.xaml.cs
        private async void Launch(object sender, RoutedEventArgs e)
        {
            Installer.Download();

            if (Process.GetProcessesByName("Minecaft.Windows").Length != 0) return;

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = startInfo.FileName = @"shell:appsFolder\Microsoft.MinecraftUWP_8wekyb3d8bbwe!App";
            p.StartInfo = startInfo;
            p.Start();

            while (true)
            {
                if (Process.GetProcessesByName("Minecraft.Windows").Length == 0) continue;
                Minecraft = Process.GetProcessesByName("Minecraft.Windows")[0];
                break;
            }

            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Logger.LogInfo("Waiting for Minecraft to load...");
                });
                while (true)
                {
                    Minecraft?.Refresh();
                    if (Minecraft is { Modules.Count: > 160 }) break;
                    Thread.Sleep(4000);
                }
            });

            Injector.Inject($@"{roamingDirectory}\Luconia\luconia.dll");
        }

        private async void LaunchCustomDll(object sender, RoutedEventArgs e)
        {
            string roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            Installer.Download();

            Logger.LogInfo("User is selecting a custom DLL...");

            OpenFileDialog openFileDialog = new()
            {
                Filter = "DLL files (*.dll)|*.dll|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != true) return;

            if (Process.GetProcessesByName("Minecaft.Windows").Length != 0) return;

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = startInfo.FileName = @"shell:appsFolder\Microsoft.MinecraftUWP_8wekyb3d8bbwe!App";
            p.StartInfo = startInfo;
            p.Start();

            while (true)
            {
                if (Process.GetProcessesByName("Minecraft.Windows").Length == 0) continue;
                Minecraft = Process.GetProcessesByName("Minecraft.Windows")[0];
                break;
            }

            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Logger.LogInfo("Waiting for Minecraft to load...");
                });
                while (true)
                {
                    Minecraft?.Refresh();
                    if (Minecraft is { Modules.Count: > 160 }) break;
                    Thread.Sleep(4000);
                }
            });

            Injector.Inject(openFileDialog.FileName);
        }

        private void GithubOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Luconia/Launcher",
                UseShellExecute = true
            });
        }

        private void DiscordOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://discord.gg/luconia",
                UseShellExecute = true
            });
        }

        private async void LaunchCustomDll(object sender, RoutedEventArgs e)
        {
            if (isInjected) return;
            string roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            Installer.Download();

            Logger.LogInfo("User is selecting a custom DLL...");

            OpenFileDialog openFileDialog = new()
            {
                Filter = "DLL files (*.dll)|*.dll|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != true) return;

            if (Process.GetProcessesByName("Minecaft.Windows").Length != 0) return;

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = startInfo.FileName = @"shell:appsFolder\Microsoft.MinecraftUWP_8wekyb3d8bbwe!App";
            p.StartInfo = startInfo;
            p.Start();

            while (true)
            {
                if (Process.GetProcessesByName("Minecraft.Windows").Length == 0) continue;
                Minecraft = Process.GetProcessesByName("Minecraft.Windows")[0];
                break;
            }

            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Logger.LogInfo("Waiting for Minecraft to load...");
                });
                while (true)
                {
                    Minecraft?.Refresh();
                    if (Minecraft is { Modules.Count: > 160 }) break;
                    Thread.Sleep(4000);
                }
            });

            Injector.Inject(openFileDialog.FileName);
            isInjected = true;
        }
    }
}
