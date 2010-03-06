using System.Collections.Generic;
using System.IO;

namespace StorEvil.Infrastructure
{
    public class Filesystem : IFilesystem
    {
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string GetFileText(string path)
        {
            return File.ReadAllText(path);
        }

        public IEnumerable<string> GetFilesInFolder(string path)
        {
            return Directory.GetFiles(path);
        }

        public IEnumerable<string> GetSubFolders(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}