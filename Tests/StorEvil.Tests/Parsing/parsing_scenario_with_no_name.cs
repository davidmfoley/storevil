using System.Linq;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.Parsing_story_text
{
    [TestFixture]
    public class parsing_scenario_with_no_name
    {
        private Story Result;
        private const string testMultiStoryText = @"
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

            Result = parser.Parse(testMultiStoryText, null);

        }
        [Test]
        public void Should_Handle_Multiple_Scenarios()
        {

            var scenario = Result.Scenarios.First();
            scenario.Name.Length.ShouldBeGreaterThan(0);
        }
    }
}