using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StorEvil.Context
{
    internal class ContextMemberNameSplitter
    {
        private static readonly Regex SplitMemberNameRegex = new Regex("[A-Z]?[a-z]*");

        public IEnumerable<string> SplitMemberName(string name)
        {
            if (name.Contains("_"))
            {
                foreach (var s in name.Split('_'))
                    yield return s;
            }
            else
            {
                foreach (Match m in SplitMemberNameRegex.Matches(name))
                    if (!string.IsNullOrEmpty(m.Value))
                        yield return m.Value;
            }
        }
    }
}