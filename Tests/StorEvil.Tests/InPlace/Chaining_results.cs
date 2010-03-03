using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class Chaining_results : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "sub context property should be true";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Should_succeed()
        {
            ResultListener.AssertWasCalled(x => x.Success(TestScenario, ScenarioText));
        }
    }
}