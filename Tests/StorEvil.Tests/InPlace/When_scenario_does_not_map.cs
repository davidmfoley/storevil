using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class When_scenario_does_not_map : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "When scenario test does not map";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener()
        {
            ResultListener.AssertWasCalled(x => x.CouldNotInterpret(TestScenario, ScenarioText));
        }
    }
}