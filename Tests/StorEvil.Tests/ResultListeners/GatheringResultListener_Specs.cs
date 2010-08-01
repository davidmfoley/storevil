using System.Linq;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.ResultListeners;
using StorEvil.Utility;

namespace StorEvil.ResultListeners.GatheringResultListener_Specs
{
    [TestFixture]
    public abstract class GatheringResultListener_Spec
    {
        protected TestGatheringResultListener Listener;

        [SetUp]
        public void SetupContext()
        {
            Listener = new TestGatheringResultListener();
            SimulateRunner();
            Listener.Handle(new SessionFinishedEvent());
        }

        protected abstract void SimulateRunner();

        protected void SimulateFailedScenario()
        {
            var failureScenario = new Scenario();
            Listener.ScenarioStarting(failureScenario);
            Listener.ScenarioFailed(new ScenarioFailureInfo(failureScenario, "success part", "failed part", "failure message"));
        }

        protected void SimulatePendingScenario()
        {
            var pendingScenario = new Scenario();
            Listener.ScenarioStarting(pendingScenario);
            Listener.ScenarioPending(new ScenarioPendingInfo(pendingScenario, "could not inperpret message"));
        }

        protected void SimulateSuccessfulScenario(string id, string name, string[] lines)
        {
            var successScenario = new Scenario(id, name, lines.Select(l=> new ScenarioLine{Text = l}).ToArray());
            Listener.ScenarioStarting(successScenario);

            foreach (var line in lines)
                Listener.Success(successScenario, line);

            Listener.ScenarioSucceeded(successScenario);
        }

        protected void SimulateStoryStarting(string storyId, string storySummary)
        {
            Listener.StoryStarting(new Story(storyId, storySummary, new[] {new Scenario()}));
        }

        protected StoryResult FirstStory()
        {
            return Listener.TestResult.Stories.First();
        }

        protected ScenarioResult FirstScenario()
        {
            return FirstStory().Scenarios.First();
        }
    }

    [TestFixture]
    public class When_no_stories_are_executed : GatheringResultListener_Spec
    {
        protected override void SimulateRunner()
        {
        }

        [Test]
        public void Result_has_zero_stories()
        {
            Listener.TestResult.Stories.Count().ShouldEqual(0);
        }
    }

    [TestFixture]
    public class When_a_sucessful_story_is_executed : GatheringResultListener_Spec
    {
        protected override void SimulateRunner()
        {
            SimulateStoryStarting("storyId", "storySummary");
            SimulateSuccessfulScenario("scenario-id", "scenario-name", new[] {"line1", "line2"});
        }

        [Test]
        public void Result_has_one_story()
        {
            Listener.TestResult.Stories.Count().ShouldEqual(1);
        }

        [Test]
        public void Story_id_is_set()
        {
            FirstStory().Id.ShouldEqual("storyId");
        }

        [Test]
        public void Story_summary_is_set()
        {
            FirstStory().Summary.ShouldEqual("storySummary");
        }

        [Test]
        public void Story_has_one_scenario()
        {
            FirstStory().Scenarios.Count().ShouldEqual(1);
        }

        [Test]
        public void Scenario_status_is_set()
        {
            FirstStory().Scenarios.First().Status.ShouldEqual(ScenarioStatus.Passed);
        }

        [Test]
        public void Scenario_has_two_lines()
        {
            FirstScenario().Lines.Count().ShouldEqual(2);
        }

        [Test]
        public void Scenario_lines_are_marked_sucessful()
        {
            FirstScenario().Lines.All(l => l.Status == ScenarioStatus.Passed).ShouldEqual(true);
        }

        [Test]
        public void Scenario_lines_have_correct_text()
        {
            FirstScenario().Lines.First().Text.ShouldEqual("line1");
            FirstScenario().Lines.Last().Text.ShouldEqual("line2");
        }

        [Test]
        public void Scenario_name_is_set()
        {
            FirstScenario().Name.ShouldEqual("scenario-name");
        }

        [Test]
        public void Scenario_id_is_set()
        {
            FirstScenario().Id.ShouldEqual("scenario-id");
        }

        
    }

    [TestFixture]
    public class When_a_failing_story_is_executed : GatheringResultListener_Spec
    {
        protected override void SimulateRunner()
        {
            SimulateStoryStarting("storyId", "storySummary");
            var testScenario = new Scenario();
            Listener.ScenarioStarting(testScenario);
            Listener.ScenarioFailed(new ScenarioFailureInfo(testScenario, "success-part", "failed-part", "failure-message"));
        }

        [Test]
        public void Result_has_one_story()
        {
            Listener.TestResult.Stories.Count().ShouldEqual(1);
        }

        [Test]
        public void Story_has_one_scenario()
        {
            FirstStory().Scenarios.Count().ShouldEqual(1);
        }

        [Test]
        public void Scenario_status_is_set_to_failed()
        {
            FirstStory().Scenarios.First().Status.ShouldEqual(ScenarioStatus.Failed);
        }

        [Test]
        public void Scenario_has_two_lines()
        {
            FirstScenario().Lines.Count().ShouldEqual(2);
        }

        [Test]
        public void First_line_is_marked_sucessful()
        {
            FirstScenario().Lines.First().Status.ShouldEqual(ScenarioStatus.Passed);
        }

        [Test]
        public void Second_line_is_marked_failed()
        {
            FirstScenario().Lines.Last().Status.ShouldEqual(ScenarioStatus.Failed);
        }

        [Test]
        public void Scenario_lines_have_correct_text()
        {
            FirstScenario().Lines.First().Text.ShouldEqual("success-part");
            FirstScenario().Lines.Last().Text.ShouldEqual("failed-part");
        }

        [Test]
        public void Scenario_failure_message_should_be_Set()
        {
            FirstScenario().FailureMessage.ShouldEqual("failure-message");
        }
    }

    [TestFixture]
    public class When_a_story_that_can_not_be_interpreted_is_executed : GatheringResultListener_Spec
    {
        private const string TestSuggestion = "void test_suggestion() {}";

        protected override void SimulateRunner()
        {
            SimulateStoryStarting("storyId", "storySummary");
            var testScenario = new Scenario();
            Listener.ScenarioStarting(testScenario);
            Listener.ScenarioPending(new ScenarioPendingInfo(testScenario, "foo bar baz", TestSuggestion));
        }

        [Test]
        public void Scenario_status_is_set_to_pending()
        {
            FirstScenario().Status.ShouldEqual(ScenarioStatus.Pending);
        }

        [Test]
        public void Line_is_added_with_pending_status()
        {
            FirstScenario().Lines.Count().ShouldEqual(1);
            FirstScenario().Lines.First().Status.ShouldEqual(ScenarioStatus.Pending);
        }

        [Test]
        public void Suggestion_is_populated()
        {
            FirstScenario().Suggestion.ShouldEqual(TestSuggestion);
        }
    }

    [TestFixture]
    public class When_multiple_scenarios_exist_for_a_story : GatheringResultListener_Spec
    {
        protected override void SimulateRunner()
        {
            SimulateStoryStarting("storyId", "storySummary");
            var pendingScenario = new Scenario();
            Listener.ScenarioStarting(pendingScenario);
            Listener.ScenarioPending(new ScenarioPendingInfo(pendingScenario, "foo bar baz"));

            var failedScenario = new Scenario();
            Listener.ScenarioStarting(failedScenario);
            Listener.ScenarioFailed(new ScenarioFailureInfo(failedScenario, "success-part", "failed-part", "failure-message"));

            SimulateSuccessfulScenario("scenario-id", "scenario-name", new[] {"line1", "line2"});
        }

        [Test]
        public void Story_has_three_scenarios()
        {
            FirstStory().Scenarios.Count().ShouldEqual(3);
        }

        [Test]
        public void Story_has_pending_scenarios_is_true()
        {
            FirstStory().HasAnyScenarios(ScenarioStatus.Pending).ShouldEqual(true);
        }

        [Test]
        public void Result_has_pending_scenarios_is_true()
        {
            Listener.TestResult.HasAnyScenarios(ScenarioStatus.Pending).ShouldEqual(true);
        }

        [Test]
        public void Total_story_fields_are_set()
        {
            Listener.TestResult.StoryCount.ShouldEqual(1);
        }
    }

    public class TestGatheringResultListener : GatheringResultListener
    {
        public TestGatheringResultListener() : base(new TestGatheredResultHandler())
        {
        }

        public GatheredResultSet TestResult
        {
            get { return ((TestGatheredResultHandler) Handler).TestResult; }
        }
    }

    public class TestGatheredResultHandler : IGatheredResultHandler
    {
        public void Handle(GatheredResultSet result)
        {
            TestResult = result;
        }

        public GatheredResultSet TestResult { get; set; }
    }
}