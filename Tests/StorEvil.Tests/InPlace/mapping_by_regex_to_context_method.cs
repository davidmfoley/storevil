using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class mapping_by_regex_to_context_method : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "Matches a regex with 42";

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
            InPlaceRunnerTestContext.RegexMatchParamValue.ShouldEqual(42);
        }
    }

    [TestFixture, Ignore("Multi-word aparams not yet supported")]
    public class matching_a_multi_word_param : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("multi-word test", new[] {ScenarioText});
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