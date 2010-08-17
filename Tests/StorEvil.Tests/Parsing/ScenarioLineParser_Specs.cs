using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
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

        [Test]
        public void Should_treat_everything_after_a_colon_as_single_word()
        {
            var words = Parser.ExtractWordsFromScenarioLine("Everything after a colon: is treated as a single word");
            words.Last().ShouldEqual("is treated as a single word");
        }

        [Test]
        public void Should_parse_numbers()
        {
            var words = Parser.ExtractWordsFromScenarioLine("numbers such as 123 can be parsed");
            words.ElementsShouldEqual("numbers", "such", "as", "123", "can", "be", "parsed");
        }

        [Test]
        public void Should_parse_negative_numbers()
        {
            var words = Parser.ExtractWordsFromScenarioLine("numbers such as -123 and -456.98 can be parsed");
            words.ElementsShouldEqual("numbers", "such", "as", "-123", "and", "-456.98", "can", "be", "parsed");
        }
    }
}