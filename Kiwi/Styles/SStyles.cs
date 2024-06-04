using System.Windows;
using System.Windows.Media;

namespace Kiwi.Styles
{
    internal static class SStyles
    {
        static SStyles()
        {
            PurpleLinkButton = Application.Current.FindResource("PurpleLinkButton") as Style;
            RedLinkButton = Application.Current.FindResource("RedLinkButton") as Style;

            PrimaryTextBlock = Application.Current.FindResource("PrimaryTextBlock") as Style;
            PrimaryTextColor = Application.Current.FindResource("PrimaryTextColor") as SolidColorBrush;

            Color6 = Application.Current.FindResource("Color6") as SolidColorBrush;
            Color9 = Application.Current.FindResource("Color9") as SolidColorBrush;
            ColorTransparent = new SolidColorBrush(Colors.Transparent);
            ColorWhite = new SolidColorBrush(Colors.White);
            ColorBlack = new SolidColorBrush(Colors.Black);
            ColorRed = Application.Current.FindResource("ColorRed") as SolidColorBrush;
            ColorOrange = Application.Current.FindResource("ColorOrange") as SolidColorBrush;
            ColorGreen = Application.Current.FindResource("ColorGreen") as SolidColorBrush;
            ColorPurple = Application.Current.FindResource("ColorPurple") as SolidColorBrush;
        }

        public static Style PurpleLinkButton { get; private set; }
        public static Style RedLinkButton { get; private set; }

        public static Style PrimaryTextBlock { get; private set; }
        public static SolidColorBrush PrimaryTextColor { get; private set; }

        public static SolidColorBrush Color6 { get; private set; }
        public static SolidColorBrush Color9 { get; private set; }
        public static SolidColorBrush ColorTransparent { get; private set; }
        public static SolidColorBrush ColorWhite { get; private set; }
        public static SolidColorBrush ColorBlack { get; private set; }
        public static SolidColorBrush ColorRed { get; private set; }
        public static SolidColorBrush ColorOrange { get; private set; }
        public static SolidColorBrush ColorGreen { get; private set; }
        public static SolidColorBrush ColorPurple { get; private set; }
    }
}
