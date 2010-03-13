using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.ResultListeners;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class Chaining_results_that_should_result_in_Failure : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "sub context property should be false";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Should_fail()
        {
            ResultListener.AssertWasCalled(
                x => x.ScenarioFailed(Any<ScenarioFailureInfo>()));
        }
    }
}