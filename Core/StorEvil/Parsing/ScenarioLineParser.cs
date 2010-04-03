using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using StorEvil.Utility;

namespace StorEvil.Parsing
{
    public class ScenarioLineParser
    {
        private static readonly Regex ExtractWordsRegex = new Regex(@"\:.+|\"".*?\""|[A-Za-z0-9\$@\./']+");
       
        public List<string> ExtractWordsFromScenarioLine(string line)
        {
            string table = null;
            
            if (HasTable(line))
            {
                table = line.After("\r\n");
                line = line.Until("\r\n");
            }
            var words = ExtractWords(line);

            if (table != null)
                words.Add(table);

            return words;
        }

        private List<string> ExtractWords(string line)
        {
            var words = new List<String>();

            // split into words
            foreach (Match m in ExtractWordsRegex.Matches(line))
            {
                var word = m.Value;
                if (word.StartsWith("\"") && word.EndsWith("\""))
                    words.Add(word.Substring(1, word.Length-2));
                else if (word.StartsWith(":"))
                    words.Add(word.Substring(1).Trim());
                else
                    words.Add(word);
            }
            return words;
        }

        private bool HasTable(string line)
        {
            return line.Contains("\r\n|");
        }
    }
}