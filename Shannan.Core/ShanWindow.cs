using MahApps.Metro.Controls;
using Shannan.Core.Extensions;
using System.Windows;

namespace Shannan.Core
{
    public class ShanWindow : MetroWindow
    {
        public ShanWindow()
        {
            // MetroWindow
            ShowTitleBar = false;
            ShowCloseButton = false;
            ShowMinButton = false;
            ShowMaxRestoreButton = false;

            IsWindowDraggable = false;
            WindowTransitionsEnabled = false;

            // Window
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.CanMinimize;

            Left = 0;
            Top = 0;
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;

            Background = "#99000000".ToSolidColorBrush();
            AllowsTransparency = true;
        }
    }
}
