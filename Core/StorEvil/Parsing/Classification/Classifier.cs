using System.Collections.Generic;
using System.Linq;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;

namespace StorEvil.Parsing.Classification
{
    public class Classifier
    {
        private StoryParser _parser;
        private StandardScenarioInterpreter _scenarioInterpreter;
        private ScenarioContext _context;

        public Classifier(AssemblyRegistry assemblyRegistry)
        {
            _parser = new StoryParser();
            _scenarioInterpreter = new StandardScenarioInterpreter(assemblyRegistry);
            _context = new SessionContext(assemblyRegistry).GetContextForStory().GetScenarioContext();
        }

        public Classification Classify(string storyText)
        {
            ClassificationLine[] classificationLines = GetClassificationLines(storyText).ToArray();
            return new Classification { Lines = classificationLines };
        }

        private IEnumerable<ClassificationLine> GetClassificationLines(string storyText)
        {
            var story = _parser.Parse(storyText, "ignore");

            var bodyLines = story.Scenarios.SelectMany(x => x.Body);
            var scenarioLines = story.Scenarios.First().Background.Union(bodyLines);

            var allLines = new LineSplitter().Split(storyText);
            foreach (var line in allLines)
            {
                yield return new ClassificationLine { Text = line.Text, Type = GetClassificationType(line, scenarioLines, story.Scenarios) };
            }
        }

        private ClassificationTypes GetClassificationType(ScenarioLine line, IEnumerable<ScenarioLine> parsedLines, IEnumerable<IScenario> scenarios)
        {

            var parsed = parsedLines.FirstOrDefault(l => l.LineNumber == line.LineNumber);
            if (parsed != null)
            {
                var match = _scenarioInterpreter.GetChain(_context, parsed.Text);
                return match != null ? ClassificationTypes.ScenarioText : ClassificationTypes.PendingScenarioText;
            }

            if (LooksLikeAScenarioTitle(line))
                return ClassificationTypes.ScenarioTitle;

            if (LooksLikeATable(line))
                return ClassificationTypes.Table;

            return ClassificationTypes.Title;
        }

        private bool LooksLikeATable(ScenarioLine line)
        {
            var trimmed = line.Text.Trim();
            return trimmed.StartsWith("|") && trimmed.EndsWith("|");
        }

        private bool LooksLikeAScenarioTitle(ScenarioLine line)
        {
            var trimmed = line.Text.Trim();
            return trimmed.StartsWith("Scenario:") || trimmed.StartsWith("Scenario Outline:");
        }
    }
}