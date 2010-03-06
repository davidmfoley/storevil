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
                return SplitAtUnderscores(name);

            return SplitOnCamelCaseBoundaries(name);
        }

        private static IEnumerable<string> SplitOnCamelCaseBoundaries(string name)
        {
            var matches = SplitMemberNameRegex.Matches(name);

            foreach (Match m in matches)
                if (!string.IsNullOrEmpty(m.Value))
                    yield return m.Value;
        }

        private static IEnumerable<string> SplitAtUnderscores(string name)
        {
            return name.Split('_');
        }
    }
}