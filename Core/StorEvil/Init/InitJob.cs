using System;
using System.IO;
using System.Reflection;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Interpreter;

namespace StorEvil.Console
{
    public class InitJob : IStorEvilJob
    {
        public IFilesystem Filesystem { get; set; }

        public InitJob(IFilesystem filesystem)
        {
            Filesystem = filesystem;
        }

        public int Run()
        {
            WriteResource("DefaultConfig.txt", "storevil.config");

            WriteResource("DefaultSparkTemplate.spark", "default.spark");
            WriteResource("Example.feature", "Example.feature");
            WriteResource("ExampleContext.cs", "ExampleContext.cs");

            WriteResource("ReadMe.txt", "ReadMe.txt");
            System.Console.WriteLine("The StorEvil project has been initialized. You need to make some edits to the storevil.config file.");
            System.Console.WriteLine("Check the ReadMe.txt file for more info");
            return 0;
        }

        private void WriteResource(string resourceName, string destination)
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), destination);
            Filesystem.WriteFile(fileName, GetResource(resourceName), false);

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