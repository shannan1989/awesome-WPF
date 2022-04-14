using Shannan.Core.Debugging;
using System.Windows;

namespace Kiwi
{
    public partial class App : Application
    {
        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            ConsoleManager.OpenConsole();
#endif

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            MainWindow mainWindow = new MainWindow();
            mainWindow.Closed += delegate
            {
                Shutdown();
            };
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
