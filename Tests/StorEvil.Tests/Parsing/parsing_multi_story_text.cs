using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.Parsing
{
    [TestFixture]
    public class parsing_multi_story_text
    {
        private Story Result;

        private const string testMultiStoryText =
            @"
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

            Result = parser.Parse(testMultiStoryText, null);
        }

        [Test]
        public void Should_Handle_Multiple_Scenarios()
        {
            Result.Scenarios.Count().ShouldEqual(2);
        }

        [Test]
        public void Should_Parse_Lines_For_Multiple_Scenarios()
        {
            var body = ((Scenario) Result.Scenarios.ElementAt(1)).Body;

            body.Select(l=>l.Text).ElementsShouldEqual("Given some other condition", "When I take some other action",
                                     "Then I should expect some other result");
        }

        [Test]
        public void Should_Parse_Line_Numbers_For_Multiple_Scenarios()
        {
            var body = ((Scenario)Result.Scenarios.ElementAt(1)).Body;

            body.Select(l => l.LineNumber).ElementsShouldEqual(10, 11, 12);
        }

        
    }

    [TestFixture]
    public class Calculating_Line_Offsets
    {
        private LineSplitter _splitter = new LineSplitter();
        private IEnumerable<ScenarioLine> Result;

        [SetUp]
        public void SetUpContext()
        {
            Result =  _splitter.Split("123\r\n4567\n8910\n\r\n");
        }
        [Test]
        public void handles_single_line()
        {

            var first = Result.First();
            first.Text.ShouldEqual("123");            

            first.StartPosition.ShouldBe(0);
            first.Length.ShouldBe(3);
            first.EndPosition.ShouldBe(3);
        }

        [Test]
        public void handles_two_lines()
        {
           
            var second = Result.ElementAt(1);
            second.Text.ShouldEqual("4567");

            second.StartPosition.ShouldBe(5);
            second.Length.ShouldBe(4);
            second.EndPosition.ShouldBe(9);
        }

        [Test]
        public void handles_naked_newline()
        {

            var third = Result.ElementAt(2);
            third.Text.ShouldEqual("8910");

            third.StartPosition.ShouldBe(10);
            third.Length.ShouldBe(4);
            third.EndPosition.ShouldBe(14);
        }


        [Test]
        public void handles_consecutive_newlines()
        {

            var fourth = Result.ElementAt(3);
            fourth.Text.ShouldEqual("");

            fourth.StartPosition.ShouldBe(15);
            fourth.Length.ShouldBe(0);
            fourth.EndPosition.ShouldBe(15);
        }

    }
}