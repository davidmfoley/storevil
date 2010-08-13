using System.Linq;
using NUnit.Framework;
using StorEvil.Utility;

namespace StorEvil.Parsing
{
    [TestFixture]
    public class parsing_empty_story
    {
        [Test]
        public void Should_Handle_No_Scenarios()
        {
            var parser = new StoryParser();
            var s = parser.Parse("", null);

            s.Scenarios.Count().ShouldEqual(0);
        }

        [Test]
        public void Sets_location()
        {
            var parser = new StoryParser();
            var s = parser.Parse("", "C:\\foo\\bar.feature");

            s.Location.ShouldEqual("C:\\foo\\bar.feature");
        }
    }

  
}