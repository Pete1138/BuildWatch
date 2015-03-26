
using System;

namespace BuildWatch
{
    public static class Extensions 
    {
        public static string ToLongDateTime(this DateTime date)
        {
            return date.ToString("dd/MM/yy HH:mm:ss");
        }
    }
}