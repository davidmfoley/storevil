using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StorEvil.Utility
{
    public static class StringUtility
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
            var chars = s
                .Replace(' ', '_')
                .ToCharArray()
                .Where(c => char.IsLetterOrDigit(c) | c == '_').ToArray();

            return EnsureFirstCharacterLegal(chars);
        }
       
        public static string ToCSharpMethodName(this string s)
        {
            var chars = s
                .Replace('\'', '_')
                .ToCharArray()
                .Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray();

            var name =  EnsureFirstCharacterLegal(chars);
            return name.Length > 512 ? name.Substring(0, 512) : name;
        }

        private static string EnsureFirstCharacterLegal(char[] chars)
        {
            var result = new string(chars);
            if (char.IsDigit(result[0]))
                return "_" + result;

            return result;
        }
    }
}