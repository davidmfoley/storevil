using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.Parsing
{
    [TestFixture]
    public class parsing_story_with_multiple_scenario_outlines_with_examples_and_tables
    {
        private Story Result;

        private const string testOutlineTableStoryText =
            @"
Story with multiple outlines
Scenario Outline: Doing something
Test with <param> and <anotherParam>
Examples:
|param|anotherParam|
|foo|bar|
|baz|42|

Scenario Outline: Doing something else
Test with <foo> and <bar>
Examples:
|foo|bar|
|foofoo|barbar|
|foo2|bar2|

";

        [SetUp]
        public void SetupContext()
        {
            Result = new StoryParser().Parse(testOutlineTableStoryText, null);
        }

        private IEnumerable<ScenarioOutline> Outlines()
        {
            return (Result.Scenarios.OfType<ScenarioOutline>());
        }

        [Test]
        public void Should_Parse_Story_header()
        {
            Result.Summary.ShouldEqual("Story with multiple outlines");
        }

        [Test]
        public void should_create_scenario_outlines()
        {
            Outlines().Count().ShouldBe(2);
        }

        [Test]
        public void First_outline_should_have_field_names()
        {
            var fieldNames = Outlines().First().FieldNames;
            fieldNames.Count().ShouldEqual(2);
            fieldNames.First().ShouldEqual("param");
            fieldNames.Last().ShouldEqual("anotherParam");
        }

        [Test]
        public void Second_outline_should_have_field_names()
        {
            var fieldNames = Outlines().Last().FieldNames;
            fieldNames.Count().ShouldEqual(2);
            fieldNames.First().ShouldEqual("foo");
            fieldNames.Last().ShouldEqual("bar");
        }

        [Test]
        public void First_outline_should_have_values()
        {
            var examples = Outlines().First().Examples;
            examples.Count().ShouldEqual(2);
            CheckExampleRow(examples.First(), "foo", "bar");
            CheckExampleRow(examples.Last(), "baz", "42");
        }

        [Test]
        public void Second_outline_should_have_values()
        {
            var examples = Outlines().Last().Examples;
            examples.Count().ShouldEqual(2);
            CheckExampleRow(examples.First(), "foofoo", "barbar");
            CheckExampleRow(examples.Last(), "foo2", "bar2");
        }

        private void CheckExampleRow(IEnumerable<string> exampleRow, params string[] expected)
        {
            var actual = exampleRow.ToArray();

            actual.Length.ShouldEqual(2);
            for (int i = 0; i < expected.Length; i++)
                actual[i].ShouldEqual(expected[i]);
        }
    }
}