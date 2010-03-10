using System.Linq;
using NUnit.Framework;
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

            body.ElementsShouldEqual("Given some other condition", "When I take some other action",
                                     "Then I should expect some other result");
        }
    }
}