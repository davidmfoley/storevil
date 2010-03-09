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

            string table = null;
 
            if (HasTable(line))
            {
                table = line.After("\r\n");
                line = line.Until("\r\n");
            }

            // split into words
            foreach (Match m in ExtractWords.Matches(line))
            {
                words.Add(m.Value);
            }

            if (table != null)
                words.Add(table);

            return words;
        }

        private bool HasTable(string line)
        {
            return line.Contains("\r\n|");
        }
    }
}