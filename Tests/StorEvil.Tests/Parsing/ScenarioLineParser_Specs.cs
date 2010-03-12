using System.Linq;
using NUnit.Framework;
using StorEvil.Utility;

namespace StorEvil.Parsing
{
    [TestFixture]
    public class ScenarioLineParser_Specs
    {
        private ScenarioLineParser Parser;

        [SetUp]
        public void SetupContext()
        {
            Parser = new ScenarioLineParser();
        }

        [Test]
        public void Should_treat_double_quoted_string_as_a_single_word()
        {
            var words = Parser.ExtractWordsFromScenarioLine("This has a \"double quoted string\"");
            words.Last().ShouldEqual("double quoted string");
        }

        [Test]
        public void Should_parse_into_words()
        {
            var words = Parser.ExtractWordsFromScenarioLine("First second third");
            words.ElementsShouldEqual("First", "second", "third");
        }
    }
}