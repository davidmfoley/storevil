using System;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;

namespace StorEvil.Console
{
   
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var filesystem = new Filesystem();
            var reader = new FilesystemConfigReader(filesystem, new ConfigParser());

            var job = new JobFactory(reader).ParseArguments(args);
                
            job.Run();
        }
    }   

    public class DisplayUsageJob : IStorEvilJob
    {       
        public void Run()
        {
            System.Console.WriteLine("usage:");

            System.Console.WriteLine("StorEvil.exe {execute|nunit|help}");
        }
    }
}