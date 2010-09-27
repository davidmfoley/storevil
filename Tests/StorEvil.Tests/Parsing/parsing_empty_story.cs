using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Utility;

namespace StorEvil.Parsing
{
    [TestFixture]
    public class parsing_empty_story
    {
        [Test]
        public void Returns_null()
        {
            var parser = new StoryParser();
            var s = parser.Parse("", null);

            s.ShouldBeNull();
        }

       
    }

  
}