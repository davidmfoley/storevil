using System.Linq;
using System.Text;

namespace StorEvil.Utility
{
    static class StringHelpers
    {
        public static string Until(this string s, string find)
        {
            return s.Substring(0, s.IndexOf(find));
        }

        public static string After(this string s, string find)
        {
            return s.Substring(s.IndexOf(find) + find.Length);
        }

        public static string FirstWord(this string s)
        {
            return s.Split(' ').First();
        }

        public static string ReplaceFirstWord(this string line, string firstWord)
        {
            return firstWord + " " +  line.After(" ");
        }
    }
}