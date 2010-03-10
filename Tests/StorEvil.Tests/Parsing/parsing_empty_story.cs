using System.Linq;
using NUnit.Framework;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.Parsing_story_text
{
    [TestFixture]
    public class parsing_empty_story
    {
        [Test]
        public void Should_Handle_No_Scenarios()
        {
            var parser = new StoryParser();
            var s = parser.Parse("", null);

            TestExtensionMethods.ShouldEqual(s.Scenarios.Count(), 0);
        }
    }
}