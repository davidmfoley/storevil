using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class When_scenario_maps_to_context_method_action_that_fails : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "When some failing action";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener_of_failure()
        {
            ResultListener.AssertWasCalled(x => x.ScenarioFailed(Any<Scenario>(), Any<string>(), Any<string>(), Any<string>()));
        }
    }

    [TestFixture]
    public class the_word_And_should_map_to_previous_significant_word : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] { "When some action", "And some other action" });

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] { TestScenario });

            RunStory(story);
        }

        [Test]
        public void Invokes_both_methods()
        {
            InPlaceRunnerTestContext.WhenSomeOtherActionCalled.ShouldEqual(true);
        }
    }

    
}