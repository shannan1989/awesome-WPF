using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Shannan.Core.Controls
{
    public enum TextBoxType
    {
        Text = 0,
        Integer = 1,
        Decimal = 2,
        Money = 3
    }

    public class TextBoxHelper : DependencyObject
    {
        public static TextBoxType GetType(DependencyObject obj)
        {
            return (TextBoxType)obj.GetValue(TypeProperty);
        }

        public static void SetType(DependencyObject obj, TextBoxType value)
        {
            obj.SetValue(TypeProperty, value);
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.RegisterAttached("Type", typeof(TextBoxType), typeof(TextBoxHelper), new PropertyMetadata(TextBoxType.Text, OnTypePropertyChanged));

        private static void OnTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = d as TextBox;
            if (null == tb)
            {
                throw new InvalidOperationException("The property 'Type' may only be set on TextBox elements.");
            }

            tb.PreviewTextInput -= TextBox_PreviewTextInput;
            tb.PreviewKeyDown -= TextBox_PreviewKeyDown;
            if (GetType(tb) != TextBoxType.Text)
            {
                tb.PreviewTextInput += TextBox_PreviewTextInput;
                tb.PreviewKeyDown += TextBox_PreviewKeyDown;
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (GetType(tb) == TextBoxType.Integer)
            {
                Regex re = new Regex("[^0-9]+");
                if (re.IsMatch(e.Text))
                {
                    e.Handled = true;
                    return;
                }
            }
            if (GetType(tb) == TextBoxType.Decimal)
            {
                Regex re = new Regex("[^0-9.]+");
                if (re.IsMatch(e.Text))
                {
                    e.Handled = true;
                    return;
                }
                if (e.Text == ".")
                {
                    if (string.IsNullOrEmpty(tb.Text))
                    {
                        tb.Text = "0";
                        tb.CaretIndex = 1;
                    }
                    else if (tb.Text.Contains("."))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            if (GetType(tb) == TextBoxType.Money)
            {
                Regex re = new Regex("[^0-9.]+");
                if (re.IsMatch(e.Text))
                {
                    e.Handled = true;
                    return;
                }
                if (e.Text == ".")
                {
                    if (string.IsNullOrEmpty(tb.Text))
                    {
                        tb.Text = "0";
                        tb.CaretIndex = 1;
                    }
                    else if (tb.Text.Contains("."))
                    {
                        e.Handled = true;
                        return;
                    }
                }
                else
                {
                    if (tb.Text.Contains("."))
                    {
                        if (tb.Text.Length > tb.Text.IndexOf(".") + 2)
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                }
            }
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (GetType(tb) == TextBoxType.Integer)
            {
                if (e.Key == Key.Space)
                {
                    e.Handled = true;
                    return;
                }
            }
            if (GetType(tb) == TextBoxType.Decimal)
            {
                if (e.Key == Key.Space)
                {
                    e.Handled = true;
                    return;
                }
            }
            if (GetType(tb) == TextBoxType.Money)
            {
                if (e.Key == Key.Space)
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}
