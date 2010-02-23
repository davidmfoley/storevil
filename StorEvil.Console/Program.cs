namespace StorEvil.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var filesystem = new Filesystem();
            var reader = new FilesystemConfigReader(filesystem, new ConfigParser(filesystem));

            var job = new ArgParser(reader).ParseArguments(args);
                
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