using System;

namespace Shannan.Core.Extensions
{
    public static class DateTimeExtension
    {
        public static int ToUnixTimestamp(this DateTime self)
        {
            return (int)(self.Ticks / 10000000 - 62135596800);
        }
    }
}
