using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Shannan.Core.Extensions
{
    public static class StringExtension
    {
        public static ImageSource ToImageSource(this string self)
        {
            return new BitmapImage(new Uri(self));
        }

        public static ImageSource ToLocalImageSource(this string self)
        {
            return new BitmapImage(new Uri(self, UriKind.RelativeOrAbsolute));
        }

        public static SolidColorBrush ToSolidColorBrush(this string self)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(self));
        }

        public static string ToMD5(this string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                s.Append(data[i].ToString("x2"));
            }
            return s.ToString();
        }

        public static string UnEscape(this string str)
        {
            return str.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");
        }
    }
}
