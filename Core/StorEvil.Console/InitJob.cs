using System;
using System.IO;
using StorEvil.Core;
using StorEvil.Infrastructure;

namespace StorEvil.Console
{
    public class InitJob : IStorEvilJob
    {
        public IFilesystem Filesystem { get; set; }

        public InitJob(IFilesystem filesystem)
        {
            Filesystem = filesystem;
        }

        public void Run()
        {
            WriteResource("DefaultConfig.txt", "storevil.config");

            WriteResource("DefaultSparkTemplate.spark", "default.spark");
            WriteResource("Example.feature", "Example.feature");
            WriteResource("ExampleContext.cs", "ExampleContext.cs");
        }

        private void WriteResource(string resourceName, string destination)
        {
            Filesystem.WriteFile(Path.Combine(Directory.GetCurrentDirectory(),destination), GetResource(resourceName), false);
        }

        private string GetResource(string resourceName)
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream("StorEvil.Console.Resources." + resourceName))
            {
                if (stream == null)
                {
                    throw new Exception("Could not find resource '" + resourceName +"'");
                }
                using(var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}