using System.Linq;

namespace StorEvil
{
    static class Helpers
    {
        public static string After(this string s, string find)
        {
            return s.Substring(s.IndexOf(find) + 1);
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