using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Shannan.Core.Extensions
{
    public static class AnimationExtension
    {
        public static void BeginDoubleAnimation(this UIElement element, DependencyProperty dp, double to, TimeSpan duration, EventHandler handler)
        {
            if (dp == null)
            {
                throw new ArgumentNullException("DependencyProperty dp can not be null");
            }

            DoubleAnimation a = new DoubleAnimation(to, new Duration(duration), FillBehavior.HoldEnd);
            a.Completed += delegate
            {
                element.BeginAnimation(dp, null);
                element.SetValue(dp, to);
            };
            if (handler != null)
            {
                a.Completed += handler;
            }
            element.BeginAnimation(dp, a);
        }

        public static void BeginDoubleAnimation(this Transform transform, DependencyProperty dp, double to, TimeSpan duration, EventHandler handler)
        {
            if (dp == null)
            {
                throw new ArgumentNullException("DependencyProperty dp can not be null");
            }

            DoubleAnimation a = new DoubleAnimation(to, new Duration(duration), FillBehavior.HoldEnd);
            a.Completed += delegate
            {
                transform.BeginAnimation(dp, null);
                transform.SetValue(dp, to);
            };
            if (handler != null)
            {
                a.Completed += handler;
            }
            transform.BeginAnimation(dp, a);
        }
    }
}
