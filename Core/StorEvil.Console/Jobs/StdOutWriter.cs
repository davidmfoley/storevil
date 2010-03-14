using StorEvil.Infrastructure;

namespace StorEvil.Console
{
    internal class StdOutWriter : ITextWriter
    {
        public void Write(string s)
        {
            System.Console.WriteLine(s);
        }
    }
}