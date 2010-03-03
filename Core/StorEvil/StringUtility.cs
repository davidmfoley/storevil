using System;
using System.Text;

namespace StorEvil
{
    internal class StringUtility
    {
        public static string StripNonNumericFormatting(string s)
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