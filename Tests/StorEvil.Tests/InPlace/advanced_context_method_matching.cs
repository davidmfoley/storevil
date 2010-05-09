using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class mapping_by_regex_to_context_method
        : InPlace.mapping_by_regex_to_context_method, UsingCompiledRunner { }

    [TestFixture]
    public class matching_a_multi_word_param
        : InPlace.matching_a_multi_word_param, UsingCompiledRunner { }
}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class mapping_by_regex_to_context_method
        : InPlace.mapping_by_regex_to_context_method, UsingNonCompiledRunner { }

    [TestFixture]
    public class matching_a_multi_word_param
        : InPlace.matching_a_multi_word_param, UsingNonCompiledRunner { }
}

namespace StorEvil.InPlace
{

    public abstract class mapping_by_regex_to_context_method : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = BuildScenario("test", 
            "Matches a regex with 42",
            "then RegEx match param value should be 42");

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener_of_1st_line_success()
        {
            AssertLineSuccess("Matches a regex with 42");

        }

        [Test]
        public void Notifies_listener_of_2nd_line_success()
        {
            AssertLineSuccess("then RegEx match param value should be 42");
        }

        [Test]
        public void Scenario_should_be_scucessful()
        {
            AssertScenarioSuccess();
        }
       
    }

    public abstract class matching_a_multi_word_param : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private  Scenario TestScenario;
        private string[] ScenarioText;
        [SetUp]
        public void SetupContext()
        {
            ScenarioText = new string[] { "test foo bar baz with multiple words", "multi word param should be \"foo bar baz\"" };

            TestScenario = BuildScenario("multi-word test", ScenarioText);
       
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener_of_success()
        {
            AssertLineSuccess("test foo bar baz with multiple words");
           
        }

        [Test]
        public void invokes_method_with_correct_param()
        {
            AssertLineSuccess("multi word param should be \"foo bar baz\"");

        }
    }
}