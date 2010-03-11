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
            var configDestination = Path.Combine(Directory.GetCurrentDirectory(), "storevil.config");
            WriteResource("Resources.DefaultConfig.txt", configDestination);

            var sparkTemplateDestination = Path.Combine(Directory.GetCurrentDirectory(), "default.spark");
            WriteResource("Resources.DefaultSparkTemplate.spark", sparkTemplateDestination);
        }

        private void WriteResource(string resourceName, string destination)
        {
            Filesystem.WriteFile(destination, GetResource(resourceName), false);
        }

        private string GetResource(string resourceName)
        {
            using(var stream = GetType().Assembly.GetManifestResourceStream("StorEvil.Console." + resourceName))
            {
                using(var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}