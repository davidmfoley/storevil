namespace StorEvil.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var job =
                new ArgParser().ParseArguments(args);

            job.Run();
        }
    }

    public class DisplayUsageJob : IStorEvilJob
    {
        public void Run()
        {
            System.Console.WriteLine("usage:");

            System.Console.WriteLine(
                "StorEvil.exe {path to context assembly} {path to story folder} {path to fixture output file}");
        }
    }
}