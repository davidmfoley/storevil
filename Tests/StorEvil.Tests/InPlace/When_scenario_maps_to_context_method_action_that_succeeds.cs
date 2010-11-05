using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class When_scenario_maps_to_context_method_action_that_succeeds
        : InPlace.When_scenario_maps_to_context_method_action_that_succeeds, UsingCompiledRunner
    {
    }
}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class When_scenario_maps_to_context_method_action_that_succeeds
        : InPlace.When_scenario_maps_to_context_method_action_that_succeeds, UsingNonCompiledRunner
    {
    }
}

namespace StorEvil.InPlace
{

    public abstract class When_scenario_maps_to_context_method_action_that_succeeds : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = BuildScenario("test", ScenarioText);
        private static string[] ScenarioText = new []
                                                   {
                                                       "When some action",
                                                       "then some action was called should be true"
                                                   };

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener_of_first_line_success()
        {
            AssertLineSuccess("When some action");
        }

       

        [Test]
        public void Notifies_listener_of_second_line_success()
        {
            AssertLineSuccess("then some action was called should be true");                  
        }

        [Test]
        public void Notifies_listener_of_scenario_success()
        {
            AssertScenarioSuccess();            
        }
    }
}