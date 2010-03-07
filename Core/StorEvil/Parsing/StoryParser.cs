using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Core;

namespace StorEvil.Parsing
{
    public class StoryParser : IStoryParser
    {
        public Story Parse(string storyText, string id)
        {
            var storyId = id ?? Guid.NewGuid().ToString().Trim();
            var scenarios = new List<IScenario>();
            var storyName = new StringBuilder();
            var lines = ParseLines(storyText);

            Action<string> handler = x => storyName.Append(x + " ");
            ScenarioBuildingInfo currentScenario = null;

            Action addScenarioOutline = () =>
                                            {
                                                var count = currentScenario.RowData.First().Count() - 1;
                                                var fieldNames = currentScenario.RowData.First().Take(count);
                                                var examples =
                                                    currentScenario.RowData.Skip(1).Select(x => x.Take(count));
                                                scenarios.Add(
                                                    new ScenarioOutline(storyId + "- outline -" + scenarios.Count,
                                                                        currentScenario.Name,
                                                                        new Scenario(storyId + "-" + scenarios.Count,
                                                                                     currentScenario.Name,
                                                                                     currentScenario.Lines), fieldNames,
                                                                        examples));
                                            };

            Action addScenario = () => scenarios.Add(new Scenario(storyId + "-" + scenarios.Count,
                                                                  currentScenario.Name,
                                                                  currentScenario.Lines));

            Action addScenarioOrOutline = () =>
                                              {
                                                  if (currentScenario != null)
                                                  {
                                                      if (currentScenario.RowData != null &&
                                                          currentScenario.RowData.Count() > 0)
                                                          addScenarioOutline();
                                                      else
                                                          addScenario();
                                                  }
                                              };

            foreach (var line in lines)
            {
                if (IsScenarioOutlineHeader(line) || IsScenarioHeader(line))
                {
                    addScenarioOrOutline();

                    currentScenario = new ScenarioBuildingInfo {Name = line.After(":").Trim()};

                    handler = GetScenarioLineHandler(currentScenario);
                }
                else if (IsStartOfExamples(line))
                {
                    handler = GetScenarioExampleRowHandler(currentScenario);
                }
                else
                {
                    handler(line.Trim());
                }
            }

            addScenarioOrOutline();

            FixEmptyScenarioNames(scenarios);

            return new Story(storyId, storyName.ToString().Trim(), scenarios);
        }

        private Action<string> GetScenarioLineHandler(ScenarioBuildingInfo currentScenario)
        {
            Action<string> handler;
            var scenario = currentScenario;
            handler = l =>
                          {
                              if (!IsComment(l))
                                  scenario.Lines.Add(l);
                          };
            return handler;
        }

        private Action<string> GetScenarioExampleRowHandler(ScenarioBuildingInfo currentScenario)
        {
            Action<string> handler;
            var scenario = currentScenario;
            handler = l =>
                          {
                              if (scenario != null && l.StartsWith("|"))
                                  scenario.RowData.Add(l.Split('|').Skip(1));
                          };
            return handler;
        }

        private void FixEmptyScenarioNames(List<IScenario> scenarios)
        {
            foreach (var scenario in scenarios)
            {
                if (!string.IsNullOrEmpty(scenario.Name))
                    continue;

                if (scenario is Scenario)
                {
                    var s = scenario as Scenario;
                    s.Name = string.Join("\r\n", s.Body.ToArray());
                }
                else if (scenario is ScenarioOutline)
                {
                    var s = scenario as ScenarioOutline;
                    s.Name = string.Join("\r\n", s.Scenario.Body.ToArray());
                }
            }
        }

        private static bool IsComment(string s)
        {
            return s.Trim().StartsWith("#");
        }

        private static bool IsStartOfExamples(string line)
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
            return text.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
        }

        internal class ScenarioBuildingInfo
        {
            public string Name;
            public List<string> Lines = new List<string>();
            public List<IEnumerable<string>> RowData = new List<IEnumerable<string>>();
        }
    }
}