using System.Linq;
using NUnit.Framework;
using StorEvil.Core;

namespace StorEvil
{
    [TestFixture]
    public class parsing_empty_story
    {
        [Test]
        public void Should_Handle_No_Scenarios()
        {
            var parser = new StoryParser();
            var s = parser.Parse("");

            s.Scenarios.Count().ShouldEqual(0);
        }
    }
    [TestFixture]
    public class parsing_simple_story_text
    {
        private Story Result;
        private const string testSingleStoryText = @"
As a user I want to do something
Scenario: Doing something
Given some condition
# this is a comment
When I take some action
Then I should expect some result
";

        [SetUp]
        public void SetupContext()
        {
            var parser = new StoryParser();
            Result = parser.Parse(testSingleStoryText);
        }

        [Test]
        public void Should_Parse_Summary()
        {
            

            Result.Summary.ShouldEqual("As a user I want to do something");
        }


        [Test]
        public void Should_Handle_Single_Scenario()
        {
           
            Result.Scenarios.Count().ShouldEqual(1);
        }

        [Test]
        public void Should_Parse_Scenario_Name()
        {
           
           ((Scenario) Result.Scenarios.First()).Name.ShouldEqual("Doing something");
        }

        [Test]
        public void Should_Parse_1st_Scenario_Text()
        {
            ((Scenario)Result.Scenarios.First()).Body.First().ShouldEqual("Given some condition");
        }

        [Test]
        public void Should_Ignore_comment_Scenario_Text()
        {
            ((Scenario)Result.Scenarios.First()).Body.Any(x=>x.Contains("comment")).ShouldEqual(false);
        }
    }
    [TestFixture]
    public class parsing_multi_story_text
    {
        private Story Result;
        private const string testMultiStoryText = @"
As a user I want to do something

Scenario: Doing something
Given some condition
When I take some action
Then I should expect some result

Scenario: Doing something else
Given some other condition
When I take some other action
Then I should expect some other result
";

        [SetUp]
        public void SetupContext()
        {
            var parser = new StoryParser();

            Result = parser.Parse(testMultiStoryText);

        }
        [Test]
        public void Should_Handle_Multiple_Scenarios()
        {

            Result.Scenarios.Count().ShouldEqual(2);
        }

        [Test]
        public void Should_Parse_Lines_For_Multiple_Scenarios()
        {
           
            var body = ((Scenario)Result.Scenarios.ElementAt(1)).Body;

            body.ElementsShouldEqual("Given some other condition", "When I take some other action", "Then I should expect some other result");
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
            Result = new StoryParser().Parse(testOutlineTableStoryText);
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
            Outline().FieldNames.First().ShouldEqual("param");
            Outline().FieldNames.Last().ShouldEqual("anotherParam");
        }

        [Test]
        public void should_have_first_Example()
        {
            Outline().Examples.First().First().ShouldEqual("foo");
            Outline().Examples.First().Last().ShouldEqual("bar");
           
        }

        [Test]
        public void should_have_second_Example()
        {
            Outline().Examples.First().First().ShouldEqual("foo");
            Outline().Examples.First().Last().ShouldEqual("bar");

        }

        [Test]
        public void should_have_correct_number_of_Examples()
        {
            Outline().Examples.Count().ShouldEqual(2);
        }

        private ScenarioOutline Outline()
        {
            return (Result.Scenarios.First() as ScenarioOutline);
        }
    }

   
}
