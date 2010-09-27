using System;
using System.IO;
using System.Reflection;
using StorEvil.Interpreter;

namespace StorEvil.Infrastructure
{
    public class ResourceWriter
    {
        public IFilesystem Filesystem { get; set; }

        public ResourceWriter(IFilesystem filesystem)
        {
            Filesystem = filesystem;
        }

        public void WriteResource(string resourceName, string destination, bool overwrite)
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), destination);
            Filesystem.WriteFile(fileName, GetResource(resourceName), overwrite);

            DebugTrace.Trace("Init Job", "Wrote file: " + destination);
        }

        private string GetResource(string resourceName)
        {
            string resourcePath = "StorEvil.Resources." + resourceName;
            Assembly thisAssembly = GetType().Assembly;

            using (var stream = thisAssembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                    throw new Exception("Could not find resource '" + resourceName + "'");

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}