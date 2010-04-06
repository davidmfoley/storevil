using System.Windows.Forms;
using StorEvil.Infrastructure;

namespace StorEvil.StubGeneration
{
    public class ClipboardWriter : ITextWriter
    {
        public void Write(string suggestions)
        {
            Clipboard.SetText(suggestions);
        }
    }
}