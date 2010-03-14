using System.Text;
using StorEvil.Infrastructure;

namespace StorEvil.ResultListeners
{
    public class FakeTextWriter : ITextWriter
    {
        public string Result
        {
            get { return _result.ToString(); }
        }

        private readonly StringBuilder _result = new StringBuilder();

        public void Write(string s)
        {
            _result.Append(s);
        }
    }
}