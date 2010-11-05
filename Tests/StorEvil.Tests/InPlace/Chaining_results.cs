using NUnit.Framework;
using StorEvil.Core;
using StorEvil.Events;

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
            AssertEventRaised<ScenarioPassed>();
        }
    } 
}

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class Disposing_contexts_with_default_lifetime
        : StorEvil.InPlace.Disposing_contexts_with_default_lifetime, UsingCompiledRunner { }

    [TestFixture]
    public class Disposing_contexts_with_default_lifetime_that_throw_in_disposal
        : StorEvil.InPlace.Disposing_contexts_with_default_lifetime_that_throw_in_disposal, UsingCompiledRunner { }
    
    [TestFixture]
    public class Disposing_contexts_with_story_lifetime
        : StorEvil.InPlace.Disposing_contexts_with_story_lifetime, UsingCompiledRunner { }

}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class Disposing_contexts_with_default_lifetime
        : StorEvil.InPlace.Disposing_contexts_with_default_lifetime, UsingNonCompiledRunner { }

    [TestFixture]
    public class Disposing_contexts_with_default_lifetime_that_throw_in_disposal
        : StorEvil.InPlace.Disposing_contexts_with_default_lifetime_that_throw_in_disposal, UsingNonCompiledRunner { }
    
    [TestFixture]
    public class Disposing_contexts_with_story_lifetime
        : StorEvil.InPlace.Disposing_contexts_with_story_lifetime, UsingNonCompiledRunner
    {
        [Test]
        public void Running_a_separate_story_it_should_be_disposed()
        {
            var story = new Story("separate", "summary", new[] { BuildScenario("test1", "when a disposable context with a story lifetime is used", "story lifetime dispose calls should be 1") });
            RunStory(story);
            AssertAllScenariosSucceeded();
        }
    }

  
}
