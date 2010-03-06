using System.Collections.Generic;

namespace StorEvil.Infrastructure
{
    public interface IFilesystem
    {
        bool FileExists(string filePath);
        string GetFileText(string path);
        IEnumerable<string> GetFilesInFolder(string path);
        IEnumerable<string> GetSubFolders(string path);
    }
}