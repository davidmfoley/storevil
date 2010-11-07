using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.Parsing
{
    [TestFixture]
    public class parsing_scenario_with_no_name
    {
        private Story Result;

        private const string testStoryText =
            @"
As a user I want to do something

Scenario: 
Given some condition
When I take some action
Then I should expect some result
";

        [SetUp]
        public void SetupContext()
        {
            var parser = new StoryParser();

            Result = parser.Parse(testStoryText, null);
        }

        [Test]
        public void Should_Handle_Multiple_Scenarios()
        {
            var scenario = Result.Scenarios.First();
            scenario.Name.Length.ShouldBeGreaterThan(0);
        }
    }

    [TestFixture]
    public class parsing_scenario_with_background
    {
        private Story Result;

        private const string testBackgroundText =
            @"
As a user I want to do something

Background:
Given some condition

Scenario: 
When I take some action
Then I should expect some result

Scenario: 
When I take some other action
Then I should expect some other result
";

        [SetUp]
        public void SetupContext()
        {
            var parser = new StoryParser();

            Result = parser.Parse(testBackgroundText, "C:\\foo\\bar.feature");
        }

        [Test]
        public void should_set_background()
        {

            var scenario = Result.Scenarios.First() as Scenario;
            scenario.ShouldNotBeNull();
            scenario.Background.Count().ShouldBe(1);
        }

        [Test]
        public void should_set_location()
        {
            var scenario = Result.Scenarios.First();
            var location = scenario.Location;
            location.Path.ShouldEqual("C:\\foo\\bar.feature");
            location.FromLine.ShouldEqual(8);
            location.ToLine.ShouldEqual(9);
        }
    }
}