using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Shannan.Core
{
    internal class NativeMethods
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        internal static extern bool AllocConsole();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        internal static extern bool FreeConsole();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetConsoleWindow();
    }
}
