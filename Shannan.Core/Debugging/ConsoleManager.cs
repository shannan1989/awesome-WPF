using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Shannan.Core.Debugging
{
    public static class ConsoleManager
    {
        public static bool HasConsole
        {
            get
            {
                return NativeMethods.GetConsoleWindow() != IntPtr.Zero;
            }
        }

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void OpenConsole()
        {
            if (!HasConsole)
            {
                NativeMethods.AllocConsole();
                InvalidateOutAndError();

                Console.WindowHeight = 40;
                Console.WindowWidth = 120;
                Console.BufferWidth = 120;
                Console.Title = "> Debug Console";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorVisible = false;
                Console.Beep();

                Log("Debug Console Waiting For Output ...");
            }
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void CloseConsole()
        {
            if (HasConsole)
            {
                SetOutAndErrorNull();
                NativeMethods.FreeConsole();
            }
        }

        public static void TriggerConsole()
        {
            if (HasConsole)
            {
                CloseConsole();
            }
            else
            {
                OpenConsole();
            }
        }

        public static void Log(string format, params object[] args)
        {
#if DEBUG
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + format, args);
#endif
        }

        public static void Log(string value)
        {
#if DEBUG
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + value);
#endif
        }

        public static void Log(int value)
        {
#if DEBUG
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + value);
#endif
        }

        private static void InvalidateOutAndError()
        {
            Type type = typeof(Console);

            FieldInfo _out = type.GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(_out != null);
            _out.SetValue(null, null);

            FieldInfo _error = type.GetField("_error", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(_error != null);
            _error.SetValue(null, null);

            MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(_InitializeStdOutError != null);
            _InitializeStdOutError.Invoke(null, new object[] { true });
        }

        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}
