using System.Linq;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.Parsing_story_text
{
    [TestFixture]
    public class Parsing_tables_of_data_in_a_scenario
    {
        private Story Result;
        private const string testOutlineTableStoryText = @"
Scenario that has a table of data.
Scenario: Doing something
Given the following data:
|1|2|3|
|4|5|6|
|7|8|9|
|10|11|12|
";

        [SetUp]
        public void SetupContext()
        {
            Result = new StoryParser().Parse(testOutlineTableStoryText, null);
        }

        [Test]
        public void Parses_table()
        {
            var scenario = Result.Scenarios.First() as Scenario;

            scenario.Body.First().ShouldContain(
@"|1|2|3|
|4|5|6|
|7|8|9|
|10|11|12|");
        }

    }

    [TestFixture]
    public class parsing_story_text_wth_examples_and_tables
    {
        private Story Result;
        private const string testOutlineTableStoryText = @"
Multi-case table and outline example
Scenario Outline: Doing something
Test with <param> and <anotherParam>
Examples:
|param|anotherParam|
|foo|bar|
|baz|42|
";

        [SetUp]
        public void SetupContext()
        {
            Result = new StoryParser().Parse(testOutlineTableStoryText, null);
        }

        [Test]
        public void Should_Parse_Story_header()
        {
            Result.Summary.ShouldEqual("Multi-case table and outline example");
            
        }

        [Test]
        public void should_create_scenario_outline()
        {
            Outline().ShouldNotBeNull();
        }

        [Test]
        public void should_have_field_names()
        {
            TestExtensionMethods.ShouldEqual(Outline().FieldNames.First(), "param");
            TestExtensionMethods.ShouldEqual(Outline().FieldNames.Last(), "anotherParam");
        }

        [Test]
        public void should_have_first_Example()
        {
            TestExtensionMethods.ShouldEqual(Outline().Examples.First().First(), "foo");
            TestExtensionMethods.ShouldEqual(Outline().Examples.First().Last(), "bar");
           
        }

        [Test]
        public void should_have_second_Example()
        {
            TestExtensionMethods.ShouldEqual(Outline().Examples.First().First(), "foo");
            TestExtensionMethods.ShouldEqual(Outline().Examples.First().Last(), "bar");

        }

        [Test]
        public void should_have_correct_number_of_Examples()
        {
            TestExtensionMethods.ShouldEqual(Outline().Examples.Count(), 2);
        }

        private ScenarioOutline Outline()
        {
            return (Result.Scenarios.First() as ScenarioOutline);
        }
    }
}