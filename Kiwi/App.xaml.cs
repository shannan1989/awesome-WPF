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
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
