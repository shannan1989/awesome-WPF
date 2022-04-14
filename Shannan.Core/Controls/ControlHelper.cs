using Shannan.Core.Converters;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Shannan.Core.Controls
{
    public class ControlHelper : DependencyObject
    {
        public static Brush GetFocusBorderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(FocusBorderBrushProperty);
        }

        public static void SetFocusBorderBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(FocusBorderBrushProperty, value);
        }

        public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached("FocusBorderBrush", typeof(Brush), typeof(ControlHelper), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static Brush GetMouseOverBorderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBorderBrushProperty);
        }

        public static void SetMouseOverBorderBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBorderBrushProperty, value);
        }

        public static readonly DependencyProperty MouseOverBorderBrushProperty = DependencyProperty.RegisterAttached("MouseOverBorderBrush", typeof(Brush), typeof(ControlHelper), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlHelper), new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(ControlHelper), new UIPropertyMetadata(string.Empty));

        public static bool GetIsEllipseClipping(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEllipseClippingProperty);
        }

        public static void SetIsEllipseClipping(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEllipseClippingProperty, value);
        }

        public static readonly DependencyProperty IsEllipseClippingProperty = DependencyProperty.RegisterAttached("IsEllipseClipping", typeof(bool), typeof(ControlHelper), new PropertyMetadata(false, OnIsEllipseClippingPropertyChanged));

        private static void OnIsEllipseClippingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement source = d as UIElement;

            if (bool.Parse(e.NewValue.ToString()) == false)
            {
                // 如果 IsEllipseClipping 附加属性被设置为 false，则清除 UIElement.Clip 属性。
                source.ClearValue(UIElement.ClipProperty);
                return;
            }

            var ellipse = source.Clip as EllipseGeometry;
            if (source.Clip != null && ellipse == null)
            {
                // 如果 UIElement.Clip 属性被用作其他用途，则抛出异常说明问题所在。
                throw new InvalidOperationException(
                    $"{typeof(ControlHelper).FullName}.{IsEllipseClippingProperty.Name} " +
                    $"is using {source.GetType().FullName}.{UIElement.ClipProperty.Name} " +
                    "for clipping, dont use this property manually.");
            }

            ellipse = ellipse ?? new EllipseGeometry();

            // 使用绑定来根据控件的宽高更新椭圆裁剪范围。
            Binding xBinding = new Binding(FrameworkElement.ActualWidthProperty.Name)
            {
                Source = source,
                Mode = BindingMode.OneWay,
                Converter = new HalfConverter()
            };
            Binding yBinding = new Binding(FrameworkElement.ActualHeightProperty.Name)
            {
                Source = source,
                Mode = BindingMode.OneWay,
                Converter = new HalfConverter()
            };

            MultiBinding centerBinding = new MultiBinding
            {
                Converter = new SizeToPointConverter(),
            };
            centerBinding.Bindings.Add(xBinding);
            centerBinding.Bindings.Add(yBinding);

            BindingOperations.SetBinding(ellipse, EllipseGeometry.RadiusXProperty, xBinding);
            BindingOperations.SetBinding(ellipse, EllipseGeometry.RadiusYProperty, yBinding);
            BindingOperations.SetBinding(ellipse, EllipseGeometry.CenterProperty, centerBinding);

            source.Clip = ellipse;
        }
    }
}
