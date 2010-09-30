using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Context;
using StorEvil.Parsing.Classification;

namespace StorEvil.Integration
{
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
}