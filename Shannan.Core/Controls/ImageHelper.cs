using Shannan.Core.Framework;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Shannan.Core.Controls
{
    public class ImageHelper : DependencyObject
    {
        static ImageHelper()
        {
            ImageQueue.DownloadCompleted += (Image image, string url, BitmapImage bitmapImage) =>
            {
                if (GetSource(image) == url)
                {
                    image.Source = bitmapImage;

                    DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(500.0)));
                    Storyboard.SetTarget(doubleAnimation, image);
                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity", new object[0]));
                    Storyboard storyboard = new Storyboard();
                    storyboard.Children.Add(doubleAnimation);
                    storyboard.Begin();
                }
            };
        }

        public static string GetSource(DependencyObject obj)
        {
            return (string)obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, string value)
        {
            obj.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(string), typeof(ImageHelper), new PropertyMetadata(string.Empty, SourcePropertyChanged));

        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageQueue.Push(d as Image, e.NewValue.ToString());
        }
    }
}
