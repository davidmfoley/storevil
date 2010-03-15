using StorEvil.Core;

namespace StorEvil.Console
{
    public class DisplayHelpJob : IStorEvilJob
    {
        private readonly string _helpText;

        public DisplayHelpJob(string helpText)
        {
            _helpText = helpText;
        }

        public int Run()
        {
            System.Console.WriteLine(_helpText);
            return 0;
        }
    }
}