using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    internal class Utils
    {
        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool CheckNet()
        {
            return InternetGetConnectedState(out int desc, 0);
        }

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int code);

        public static bool CheckKey(int code) => (GetKeyState(code) & 0xFF00) == 0xFF00;
    }
}
