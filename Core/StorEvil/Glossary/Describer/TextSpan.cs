using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Core
{
    public class TextSpan :StepSpan
    {
        public static TextSpan Merge(IEnumerable<TextSpan> spans)
        {
            var words = spans.Select(x => x.Text).ToArray();
            var joined = string.Join("", words);
            return new TextSpan(joined);
        }

        public TextSpan(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}