using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.Integration
{
    [TestFixture]
    public class CommandLineIntegrationTests
    {
        [SetUp]
        public void SetupContext()
        {
        }
    }

    [TestFixture]
    public class Classifying_a_story
    {
        private Classification Result;

        private const string StoryText =
            @"This is a story

Scenario: A scenario with title
This is an example line of a scenario that is interpreted
This is an example line of a scenario that is not interpreted
This is a table
|foo|bar|";

        [SetUp]
        public void SetUpContext()
        {
            Result = Parse(StoryText);
        }

        private Classification Parse(string storyText)
        {
            var assemblyRegistry = new TestAssemblyRegistry();

            var classifier = new Classifier(assemblyRegistry);

            return classifier.Classify(storyText);
        }

        private class TestAssemblyRegistry : AssemblyRegistry
        {
            public override IEnumerable<Type> GetTypesWithCustomAttribute<T>()
            {
                return new[] {typeof (ClassificationTestContext)};
            }

            private class ClassificationTestContext
            {
                public void This_is_an_example_line_of_a_scenario_that_is_interpreted() {}
            }
        }

        [Test]
        public void First_line_is_title()
        {
            var first = Result.Lines.First();
            first.Type.ShouldEqual(ClassificationTypes.Title);
            first.Text.ShouldEqual("This is a story");
        }

        [Test]
        public void Scenario_title_line_is_handled()
        {
            var scenarioTitleLine = Result.Lines.ElementAt(2);
            scenarioTitleLine.Type.ShouldBe(ClassificationTypes.ScenarioTitle);
        }

        [Test]
        public void interpreted_text_is_appropriately_classified()
        {
            var scenarioTextLine = Result.Lines.ElementAt(3);
            scenarioTextLine.Type.ShouldBe(ClassificationTypes.ScenarioText);
        }

        [Test]
        public void pending_text_is_appropriately_classified()
        {
            var scenarioTextLine = Result.Lines.ElementAt(4);
            scenarioTextLine.Type.ShouldBe(ClassificationTypes.PendingScenarioText);
        }

        [Test]
        public void table_row_is_appropriately_classified()
        {
            var scenarioTextLine = Result.Lines.ElementAt(6);
            scenarioTextLine.Type.ShouldBe(ClassificationTypes.Table);
        }
    }

    internal class Classifier
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
            return new Classification {Lines = classificationLines};
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
            
            var parsed = parsedLines.FirstOrDefault(l=>l.LineNumber == line.LineNumber);
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

    internal class ClassificationLine
    {
        public ClassificationTypes Type { get; set; }

        public string Text { get; set; }
    }

    internal class Classification
    {
        public IEnumerable<ClassificationLine> Lines;
    }

    internal enum ClassificationTypes
    {
        Title,
        ScenarioTitle,
        ScenarioText,
        PendingScenarioText,
        Table
    }
}