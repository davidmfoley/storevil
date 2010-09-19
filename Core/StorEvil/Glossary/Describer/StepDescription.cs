using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Core
{
    public class StepDescription
    {
        public string Description
        {
            get { 
                var spans = Spans.Select(x => x.Text).ToArray();
                return string.Join("", spans);
            }
        }
        public string ChildDescription = "";
        public IEnumerable<StepSpan> Spans = new StepSpan[] {};
    }
}

