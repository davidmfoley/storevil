using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StorEvil.Utility
{
    static class EnumerableHelpers
    {
        public static IEnumerable<int> UpTo(this int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                yield return i;
            }
        }

        public static void Times(this int times, Action action)
        {
            for (int i = 0; i < times; i++)
            {
                action();
            }
        }

        public static void Each<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var x in list)
            {
                action(x);
            }            
        }
    }
    static class Helpers
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



    public static class PathHelper
    {
        public static string[] GetRelativePathPieces(string from, string to)
        {
            return GetPathPieces(from, to).ToArray();
        }

        private static IEnumerable<string> GetPathPieces(string from, string to)
        {            
            var fromPieces = SplitPath(from);
            var toPieces = SplitPath(to);
            
            while (fromPieces.Any() && toPieces.Any() && fromPieces.First() == toPieces.First())
            {
                fromPieces = fromPieces.Skip(1);
                toPieces = toPieces.Skip(1);
            }

            var backUps = 1.UpTo(fromPieces.Count()).Select(x=>"..");

            return backUps.Union(toPieces);
        }

        public static IEnumerable<string> SplitPath(string pathAsString)
        {
            var path = new DirectoryInfo(pathAsString);

            var pieces = new List<string>();
            
            pieces.Add(path.Name);
            while (path.Parent != null)
            {
                path = path.Parent;
                pieces.Add(path.Name);
            }

            pieces.Reverse();
            return pieces;
        }
    }
}