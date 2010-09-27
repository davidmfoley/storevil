using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StorEvil.Utility
{
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
            
            while (fromPieces.Any() && toPieces.Any() && fromPieces.First().ToLower() == toPieces.First().ToLower())
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