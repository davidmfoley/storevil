using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class mapping_by_regex_to_context_method
        : InPlace.mapping_by_regex_to_context_method, UsingCompiledRunner { }

    [TestFixture, Ignore("In progress")]
    public class matching_a_multi_word_param
        : InPlace.matching_a_multi_word_param, UsingCompiledRunner { }
}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class mapping_by_regex_to_context_method
        : InPlace.mapping_by_regex_to_context_method, UsingNonCompiledRunner { }

    [TestFixture, Ignore("In progress")]
    public class matching_a_multi_word_param
        : InPlace.matching_a_multi_word_param, UsingNonCompiledRunner { }
}

namespace StorEvil.InPlace
{

    public abstract class mapping_by_regex_to_context_method : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = BuildScenario("test", "Matches a regex with 42");

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener_of_success()
        {
            ResultListener.AssertWasCalled(x => x.Success(TestScenario, "Matches a regex with 42"));
        }

        [Test]
        public void invokes_method_with_correct_param()
        {
            InPlaceRunnerTestContext.RegexMatchParamValue.ShouldEqual(42);
        }
    }

    [TestFixture, Ignore("In progress")]
    public class matching_a_multi_word_param : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = BuildScenario("multi-word test", ScenarioText);
        private const string ScenarioText = "test foo bar baz with multiple words";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener_of_success()
        {
            ResultListener.AssertWasCalled(x => x.Success(TestScenario, ScenarioText));
        }

        [Test]
        public void invokes_method_with_correct_param()
        {
            InPlaceRunnerTestContext.MultiWordParam.ShouldEqual("foo bar baz");
        }
    }
}