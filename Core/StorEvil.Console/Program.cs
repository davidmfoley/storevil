using System;
using Funq;
using StorEvil.Configuration;
using StorEvil.Context;
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
                
            var result = job.Run();
            Environment.Exit(result);
        }
    }   

    public class DisplayUsageJob : IStorEvilJob
    {       
        public int Run()
        {
            System.Console.WriteLine("usage:");

            System.Console.WriteLine("StorEvil.exe {execute|nunit|help}");
            return 0;
        }
    }
}