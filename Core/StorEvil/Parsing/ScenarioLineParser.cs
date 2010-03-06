using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StorEvil.Parsing
{
    public class ScenarioLineParser
    {
        private static readonly Regex ExtractWords = new Regex(@"[A-Za-z0-9\$@\./]+");

        public List<string> ExtractWordsFromScenarioLine(string line)
        {
            var words = new List<String>();

            // split into words
            foreach (Match m in ExtractWords.Matches(line))
            {
                words.Add(m.Value);
            }

            return words;
        }
    }
}