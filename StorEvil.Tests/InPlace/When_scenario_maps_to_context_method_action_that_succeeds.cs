using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class When_scenario_maps_to_context_method_action_that_succeeds : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "When some action";

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
        public void invokes_method()
        {
            InPlaceRunnerTestContext.WhenSomeActionCalled.ShouldEqual(true);
        }
    }
}