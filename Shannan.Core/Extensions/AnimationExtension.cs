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

        /// <summary>
        /// 使元素持续旋转
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="duration">旋转时长</param>
        public static void RotateForever(this FrameworkElement element, TimeSpan duration)
        {
            element.RenderTransform = new RotateTransform();
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            DoubleAnimation a = new DoubleAnimation(0, 360, duration);
            Storyboard.SetTarget(a, element);
            Storyboard.SetTargetProperty(a, new PropertyPath("RenderTransform.Angle"));

            Storyboard storyboard = new Storyboard()
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            storyboard.Children.Add(a);
            storyboard.Begin();
        }

        public static void FadeInOut(this FrameworkElement element, TimeSpan duration)
        {
            DoubleAnimation a = new DoubleAnimation(1, duration);
            Storyboard.SetTarget(a, element);
            Storyboard.SetTargetProperty(a, new PropertyPath("Opacity"));

            Storyboard storyboard = new Storyboard()
            {
                AutoReverse = true
            };
            storyboard.Children.Add(a);
            storyboard.Begin();
        }
    }
}
