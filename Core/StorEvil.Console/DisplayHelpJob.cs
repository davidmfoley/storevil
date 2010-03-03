namespace StorEvil.Console
{
    public class DisplayHelpJob : IStorEvilJob
    {
        private readonly string _helpText;

        public DisplayHelpJob(string helpText)
        {
            _helpText = helpText;
        }

        public void Run()
        {
            System.Console.WriteLine(_helpText);
        }
    }
}