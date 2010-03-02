using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Core;

namespace StorEvil
{
    /// <summary>
    /// Parses stories... this code is a bit ugly and needs looking into
    /// </summary>
    public class StoryParser : IStoryParser
    {
        public Story Parse(string storyText)
        {
            var scenarios = new List<IScenario>();
             
            var storyName = new StringBuilder();

            var lines = ParseLines(storyText);

            Action<string> handler = x => storyName.Append(x + " ");

            ScenarioBuildingInfo currentScenario = null;

            foreach (var line in lines)
            {
                if (IsScenarioOutlineHeader(line) || IsScenarioHeader(line))
                {
                    if (currentScenario != null)
                        scenarios.Add(new Scenario(currentScenario.Name, currentScenario.Lines));

                    currentScenario = new ScenarioBuildingInfo { Name = line.After(":").Trim() };

                    ScenarioBuildingInfo scenario = currentScenario;
                    handler = l =>  {
                        if (!IsComment(l))
                            scenario.Lines.Add(l);
                    };
                }
                else if (IsStartOfExamples(line))
                {
                    handler = l =>
                                  {
                                      if (currentScenario != null && l.StartsWith("|"))
                                          currentScenario.RowData.Add(l.Split('|').Skip(1));
                                  };
                }
                else
                {
                    handler(line.Trim());
                }

            }

            if (currentScenario != null)
            {
                if (currentScenario.RowData != null && currentScenario.RowData.Count() > 0)
                {
                    var count = currentScenario.RowData.First().Count() - 1;
                    var fieldNames = currentScenario.RowData.First().Take(count);
                    var examples = currentScenario.RowData.Skip(1).Select(x=>x.Take(count));
                    scenarios.Add(new ScenarioOutline(currentScenario.Name, new Scenario(currentScenario.Name, currentScenario.Lines), fieldNames, examples));
                }
                else
                {
                    scenarios.Add(new Scenario(currentScenario.Name, currentScenario.Lines, currentScenario.RowData));
                }
            }

            return new Story(Guid.NewGuid().ToString().Trim(), storyName.ToString().Trim(), scenarios);
        }

        private bool IsComment(string s)
        {
            return s.Trim().StartsWith("#");
        }

        private bool IsStartOfExamples(string line)
        {
            return line.ToLower().StartsWith("examples:");
        }

        private static bool IsScenarioHeader(string line)
        {
            return line.ToLower().StartsWith("scenario:");
        }

        private static bool IsScenarioOutlineHeader(string line)
        {
            return line.ToLower().StartsWith("scenario outline:");
        }


        private static IEnumerable<string> ParseLines(string text)
        {
            foreach (string line in text.Split(new[] { '\n', '\r' }))
            {
                var s = line.Trim();
                if (s.Length > 0)
                    yield return s.Trim();
            }
        }
    }

    public class ScenarioBuildingInfo
    {
        public string Name;
        public List<string> Lines = new List<string>();
        public List<IEnumerable<string>> RowData = new List<IEnumerable<string>>();
    }

    
}
