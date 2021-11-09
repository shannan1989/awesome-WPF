using System.Windows;
using System.Windows.Media;

namespace Shannan.Core.Controls
{
    public class ButtonHelper : DependencyObject
    {
        public static string GetNormalIcon(DependencyObject obj)
        {
            return (string)obj.GetValue(NormalIconProperty);
        }

        public static void SetNormalIcon(DependencyObject obj, string value)
        {
            obj.SetValue(NormalIconProperty, value);
        }

        public static readonly DependencyProperty NormalIconProperty = DependencyProperty.RegisterAttached("NormalIcon", typeof(string), typeof(ButtonHelper), new PropertyMetadata(string.Empty));

        public static string GetHoverIcon(DependencyObject obj)
        {
            return (string)obj.GetValue(HoverIconProperty);
        }

        public static void SetHoverIcon(DependencyObject obj, string value)
        {
            obj.SetValue(HoverIconProperty, value);
        }

        public static readonly DependencyProperty HoverIconProperty = DependencyProperty.RegisterAttached("HoverIcon", typeof(string), typeof(ButtonHelper), new PropertyMetadata(string.Empty));

        public static string GetPressIcon(DependencyObject obj)
        {
            return (string)obj.GetValue(PressIconProperty);
        }

        public static void SetPressIcon(DependencyObject obj, string value)
        {
            obj.SetValue(PressIconProperty, value);
        }

        public static readonly DependencyProperty PressIconProperty = DependencyProperty.RegisterAttached("PressIcon", typeof(string), typeof(ButtonHelper), new PropertyMetadata(string.Empty));

        public static string GetDisableIcon(DependencyObject obj)
        {
            return (string)obj.GetValue(DisableIconProperty);
        }

        public static void SetDisableIcon(DependencyObject obj, string value)
        {
            obj.SetValue(DisableIconProperty, value);
        }

        public static readonly DependencyProperty DisableIconProperty = DependencyProperty.RegisterAttached("DisableIcon", typeof(string), typeof(ButtonHelper), new PropertyMetadata(string.Empty));

        public static Brush GetHoverBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(HoverBackgroundProperty);
        }

        public static void SetHoverBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(HoverBackgroundProperty, value);
        }

        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.RegisterAttached("HoverBackground", typeof(Brush), typeof(ButtonHelper), new PropertyMetadata(Brushes.Transparent));

        public static Brush GetDisableBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(DisableBackgroundProperty);
        }

        public static void SetDisableBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(DisableBackgroundProperty, value);
        }

        public static readonly DependencyProperty DisableBackgroundProperty = DependencyProperty.RegisterAttached("DisableBackground", typeof(Brush), typeof(ButtonHelper), new PropertyMetadata(Brushes.Transparent));
    }
}
