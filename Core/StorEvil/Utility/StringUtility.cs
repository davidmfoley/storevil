using System;
using System.Text;

namespace StorEvil
{
    internal static class StringUtility
    {
        public static string StripNonNumericFormatting(this string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (Char.IsNumber(c) || c == '.')
                    sb.Append(c);
            }

            var val = sb.ToString();
            //if (val.Contains("."))
            //    return val + "M";
            return val;
        }
    }
}