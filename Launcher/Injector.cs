using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Launcher
{
    internal class Injector
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        public const int PROCESS_CREATE_THREAD = 0x0002;
        public const int PROCESS_QUERY_INFORMATION = 0x0400;
        public const int PROCESS_VM_OPERATION = 0x0008;
        public const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_VM_READ = 0x0010;

        public const uint MEM_COMMIT = 0x00001000;
        public const uint MEM_RESERVE = 0x00002000;
        public const uint PAGE_READWRITE = 4;

        [DllImport("user32.dll")]
        public static extern int SendMessage(nint hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("kernel32.dll")]
        public static extern nint OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern nint GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern nint GetProcAddress(nint hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern nint VirtualAllocEx(nint hProcess, nint lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, uint nSize, out nuint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes, uint dwStackSize, nint lpStartAddress, nint lpParameter, uint dwCreationFlags, nint lpThreadId);

        public static void Inject(string path)
        {
            try
            {
                var infoFile = new FileInfo(path);
                var fSecurity = infoFile.GetAccessControl();
                fSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-15-2-1"), FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                infoFile.SetAccessControl(fSecurity);

                var targetProcess = Process.GetProcessesByName("Minecraft.Windows")[0];
                var procHandle = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);
                var loadLibraryAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                var allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)((path.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

                WriteProcessMemory(procHandle, allocMemAddress, Encoding.Default.GetBytes(path), (uint)((path.Length + 1) * Marshal.SizeOf(typeof(char))), out _);
                CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddress, allocMemAddress, 0, IntPtr.Zero);

                Logger.LogInfo("Successfully injected the dll");
            }
            catch (Exception e)
            {
                Logger.LogError("Injection failed");
                Logger.LogException(e);
            }
        }
    }
}
