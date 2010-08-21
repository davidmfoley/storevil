using NUnit.Framework;
using StorEvil.Core;
using StorEvil.Events;


namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class Chaining_results_that_should_result_in_Failure
        : StorEvil.InPlace.Chaining_results_that_should_result_in_Failure, UsingCompiledRunner { }

}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class Chaining_results_that_should_result_in_Failure
        : StorEvil.InPlace.Chaining_results_that_should_result_in_Failure, UsingNonCompiledRunner { }

}
namespace StorEvil.InPlace
{
    public abstract class Chaining_results_that_should_result_in_Failure : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = BuildScenario("test", new[] {"sub context property should be false"});

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Should_fail()
        {
            AssertWasRaised<LineExecuted>(x=>x.Status == ExecutionStatus.Failed);            
        }

    }
}