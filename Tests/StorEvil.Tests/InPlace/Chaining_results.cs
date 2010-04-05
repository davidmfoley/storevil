using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class Chaining_results
        : StorEvil.InPlace.Chaining_results, UsingCompiledRunner { }

}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class Chaining_results
        : StorEvil.InPlace.Chaining_results, UsingNonCompiledRunner { }

}

namespace StorEvil.InPlace
{   
    public abstract class Chaining_results : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = BuildScenario("test", new[] { ScenarioText });
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
            ResultListener.AssertWasCalled(x => x.Success(Any<Scenario>(), Any<string>()));
        }
    }
}