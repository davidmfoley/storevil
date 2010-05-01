using System;
using System.Diagnostics;
using System.IO;
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
            System.Console.Write("Check the ReadMe.txt file for more info");
            return 0;
        }

        private void WriteResource(string resourceName, string destination)
        {
            Filesystem.WriteFile(Path.Combine(Directory.GetCurrentDirectory(), destination), GetResource(resourceName),
                                 false);


            DebugTrace.Trace("Init Job", "Wrote file: " + destination);
        }

        private string GetResource(string resourceName)
        {
            using (
                var stream = GetType().Assembly.GetManifestResourceStream("StorEvil.Console.Resources." + resourceName))
            {
                if (stream == null)
                {
                    throw new Exception("Could not find resource '" + resourceName + "'");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}