using Spark;
using StorEvil.Core;
using StorEvil.Interpreter;

namespace StorEvil.Glossary
{
    public abstract class GlossaryView : AbstractSparkView
    {
        public GlossaryViewModel Model { get; set; }
        public string HTML(object h)
        {
            DebugTrace.Trace(this, "HTML:" + h);
            return h.ToString();
        }

        public string FormatDescription(StepDescription description)
        {
            // HACK
            return description.Description
                .Replace("<", "<span class=\"param\">")
                .Replace(">", "</span>");
        }
    }
}