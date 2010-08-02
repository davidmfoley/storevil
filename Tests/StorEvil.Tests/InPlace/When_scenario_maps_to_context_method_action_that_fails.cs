using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Utility;


namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class When_scenario_maps_to_context_method_action_that_fails
        : InPlace.When_scenario_maps_to_context_method_action_that_fails, UsingCompiledRunner { }

    [TestFixture]
    public class the_word_And_should_map_to_previous_significant_word
        : InPlace.the_word_And_should_map_to_previous_significant_word, UsingCompiledRunner { }
}


namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class When_scenario_maps_to_context_method_action_that_fails
        : InPlace.When_scenario_maps_to_context_method_action_that_fails, UsingNonCompiledRunner { }

    [TestFixture]
    public class the_word_And_should_map_to_previous_significant_word
        : InPlace.the_word_And_should_map_to_previous_significant_word, UsingNonCompiledRunner { }
}

namespace StorEvil.InPlace
{

    public abstract class When_scenario_maps_to_context_method_action_that_fails : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = BuildScenario("test", new[] { ScenarioText });
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
            AssertEventRaised<LineExecutedEvent>(x=>x.Status == ExecutionStatus.Failed);            
        }

        [Test]
        public void Does_not_Notify_listener_of_scenario_success()
        {
            FakeEventBus.CaughtEvents.OfType<ScenarioFinishedEvent>().Any(x => x.Status == ExecutionStatus.Passed).ShouldBe(false);
            
        }
    }

    public abstract class the_word_And_should_map_to_previous_significant_word : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = BuildScenario("test",
                                                              new[] {"When some action", "And some other action"});

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Invokes_both_methods()
        {
            AssertLineSuccess("And some other action");
        }
    }
}