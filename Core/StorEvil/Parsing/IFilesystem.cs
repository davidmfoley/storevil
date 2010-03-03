using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StorEvil
{
    public interface IFilesystem
    {
        bool FileExists(string filePath);
        string GetFileText(string path);
        IEnumerable<string> GetFilesInFolder(string path);
        IEnumerable<string> GetSubFolders(string path);
    }

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