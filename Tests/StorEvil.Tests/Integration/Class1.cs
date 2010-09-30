using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Context;
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
This is an example line of a scenario";

        [SetUp]
        public void SetUpContext()
        {
            Result = Parse(StoryText);
        }

        private Classification Parse(string storyText)
        {
            var assemblyRegistry = new AssemblyRegistry(new[] {GetType().Assembly});

            var classifier = new Classifier(assemblyRegistry);

            return classifier.Classify(storyText);
        }

        [Test]
        public void Has_one_line_per_line_in_the_story()
        {
            Result.Lines.Count().ShouldBe(4);
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
    }

    internal class Classifier
    {
        private StoryParser _parser;

        public Classifier(AssemblyRegistry assemblyRegistry)
        {
            _parser = new StoryParser();
        }

        public Classification Classify(string storyText)
        {
            

            ClassificationLine[] classificationLines = GetClassificationLines(storyText).ToArray();
            return new Classification {Lines = classificationLines};
        }

        private IEnumerable<ClassificationLine> GetClassificationLines(string storyText)
        {
            var story = _parser.Parse(storyText, "ignore");

           // var lines = story.Scenarios.SelectMany(x=>x.Background).Union(story.Scenarios.SelectMany(x=>x.))
            

            return storyText.Split(new[] {"\n", "\r\n"}, StringSplitOptions.None)
                .Select(x => new ClassificationLine
                                 {
                                     Text = x,
                                     Type = ClassificationTypes.Title
                                 });
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
        ScenarioTitle
    }
}