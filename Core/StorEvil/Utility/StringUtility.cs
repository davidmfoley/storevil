using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StorEvil.Utility
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

        public static string ToCSharpName(this string s)
        {

            var chars = s.ToCharArray().Where(c => char.IsLetterOrDigit(c) || c =='_').ToArray();
            return new string(chars);
        }

        public static string ToCSharpMethodName(this string s)
        {
            return Regex
                .Replace(s, @"\\\:\.", "_")
                .Replace("__", "_")
                .ToCSharpName();          
        }
    }
}