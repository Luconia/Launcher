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
using Path = System.IO.Path;

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

            if (!Utils.CheckNet())
            {
                Logger.LogError("No internet found!");
                MessageBox.Show("You need internet to use the launcher!", "An error has occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            installer = new Installer();

            Logger.LogInfo("Started Luconia Launcher");
            
            if (Directory.Exists($@"{roamingDirectory}\Luconia\img"))
                Directory.CreateDirectory($@"{roamingDirectory}\Luconia\img");
            
            using FileStream stream = new FileStream($"{Path.GetTempPath()}\\luconiaBG.jpg", FileMode.OpenOrCreate);
            BitmapFrame frame = BitmapFrame.Create((BitmapSource)LauncherBackground.ImageSource);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(frame);
            encoder.Save(stream);

            if (File.Exists($@"{roamingDirectory}\Luconia\img\bg.jpg"))
            {
                var customBackground = new BitmapImage();
                customBackground.BeginInit();
                customBackground.UriSource = new Uri($@"{roamingDirectory}\Luconia\img\bg.jpg", UriKind.Relative);
                customBackground.EndInit();

                LauncherBackground.ImageSource = customBackground;
            }
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
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Luconia/Launcher",
                UseShellExecute = true
            });
        }
        private void DiscordOnClick(object sender, RoutedEventArgs e)
        {
            if (Process.GetProcessesByName("Discord").Length > 0)
                Process.Start(new ProcessStartInfo
                {
                    FileName = "discord://-/invite/luconia",
                    UseShellExecute = true
                });
            else
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://discord.gg/luconia",
                    UseShellExecute = true
                });
            }
        }

        private void CustomizeWallpaperButtonOnClick(object sender, RoutedEventArgs e)
        {
            var originalWallpaper = new BitmapImage();
            originalWallpaper.BeginInit();
            originalWallpaper.UriSource = new Uri($"{Path.GetTempPath()}\\luconiaBG.jpg", UriKind.Relative);
            originalWallpaper.EndInit();

            LauncherBackground.ImageSource = originalWallpaper;
            
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Images (*.png,*.jpg,*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != true) return;

            var customBackground = new BitmapImage();
            customBackground.BeginInit();
            customBackground.UriSource = new Uri(openFileDialog.FileName, UriKind.Relative);
            customBackground.EndInit();

            LauncherBackground.ImageSource = customBackground;

            if (!Directory.Exists($@"{roamingDirectory}\Luconia\img"))
                Directory.CreateDirectory($@"{roamingDirectory}\Luconia\img");

            if (!IsFileLocked(new FileInfo($@"{roamingDirectory}\Luconia\img\bg.jpg")))
            {
                using FileStream stream = new FileStream($@"{roamingDirectory}\Luconia\img\bg.jpg", FileMode.OpenOrCreate);
                BitmapFrame frame = BitmapFrame.Create((BitmapSource)LauncherBackground.ImageSource);

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(frame);
                encoder.Save(stream);
            }
            else if (IsFileLocked(new FileInfo($@"{roamingDirectory}\Luconia\img\bg.jpg")))
                Logger.LogError($@"{roamingDirectory}\Luconia\img\bg.jpg is in use, so the launcher could not change the background!");
        }
        
        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                return true;
            }
            
            return false;
        }
    }
}
