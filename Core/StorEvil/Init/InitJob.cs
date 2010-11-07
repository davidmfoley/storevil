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
            ResourceWriter = new ResourceWriter(filesystem);
        }

        protected ResourceWriter ResourceWriter { get; set; }

        public int Run()
        {
            ResourceWriter.WriteResource("DefaultConfig.txt", "storevil.config", false);

            ResourceWriter.WriteResource("DefaultSparkTemplate.spark", "default.spark", false);
            ResourceWriter.WriteResource("Example.feature", "Example.feature", false);
            ResourceWriter.WriteResource("ExampleContext.cs", "ExampleContext.cs", false);

            ResourceWriter.WriteResource("ReadMe.txt", "ReadMe.txt", false);
            System.Console.WriteLine("The StorEvil project has been initialized. You need to make some edits to the storevil.config file.");
            System.Console.WriteLine("Check the ReadMe.txt file for more info");
            return 0;
        }

    
    }
}