using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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



namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class Disposing_contexts
        : StorEvil.InPlace.Disposing_contexts, UsingCompiledRunner { }

}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class Disposing_contexts
        : StorEvil.InPlace.Disposing_contexts, UsingNonCompiledRunner { }

}

namespace StorEvil.InPlace
{
    public abstract class Disposing_contexts : InPlaceRunnerSpec<InPlaceRunnerDisposalTestContext>
    {
        private readonly Scenario TestScenario1 = BuildScenario("test", "when a disposable context is used");
        private readonly Scenario TestScenario2 = BuildScenario("test", "then it should be disposed");
   
        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] { TestScenario1, TestScenario2 });

            RunStory(story);
        }

        [Test]
        public void Should_succeed()
        {
           AssertLineSuccess("then it should be disposed");
            
        }
    }

    [Context]
    public class InPlaceRunnerDisposalTestContext :IDisposable
    {
        private static int DisposeCalls = 0;
        public void when_a_disposable_context_is_used() {}
        public void then_it_should_be_disposed()
        {
            Assert.That(DisposeCalls, Is.GreaterThan(0));
        }

        public void Dispose()
        {
            DisposeCalls++;
        }
    }
}