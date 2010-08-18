using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.Events;

namespace StorEvil.TeamCity
{
    [TestFixture]
    public class team_city_result_listener
    {
        private TeamCityResultListener listener;
        private ITeamCityMessageWriter messageWriter;

        [SetUp]
        public void SetUp()
        {
            messageWriter = MockRepository.GenerateStub<ITeamCityMessageWriter>();
            listener = new TeamCityResultListener(messageWriter);
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_story_starts_being_executed()
        {
            listener.Handle(new StoryStarting() {Story = GetStory("Feature   : my \n summary")});

            messageWriter.AssertWasCalled(mw => mw.Write("##teamcity[testSuiteStarted name='my summary']"));
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_story_execution_is_finished()
        {
            listener.Handle(new StoryFinished() { Story = GetStory("Feature   : my \n summary \n    ")});

            messageWriter.AssertWasCalled(mw => mw.Write("##teamcity[testSuiteFinished name='my summary']"));
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_scenario_starts_being_executed()
        {
            listener.Handle(new ScenarioStarting() { Scenario = GetScenario("my scenario") });

            messageWriter.AssertWasCalled(mw => mw.Write("##teamcity[testStarted name='my scenario']"));
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_scenario_execution_passed()
        {
            listener.Handle(new ScenarioFinished() { Scenario = GetScenario("my scenario"), Status = ExecutionStatus.Passed});

            messageWriter.AssertWasCalled(mw => mw.Write("##teamcity[testFinished name='my scenario']"));
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_scenario_execution_failed()
        {
            listener.Handle(new LineExecuted() { Scenario = GetScenario("my scenario"), Status = ExecutionStatus.Failed, Message = "error message [ ] ' \r" });

            messageWriter.AssertWasCalled(mw => mw.Write("##teamcity[testFailed name='my scenario' message='error message |[ |] |' |r' details='error message |[ |] |' |r']"));
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_scenario_execution_is_inconclusive()
        {
            listener.Handle(new LineExecuted() { Scenario = GetScenario("my scenario"), Status = ExecutionStatus.Pending, Suggestion = "my suggestion" });

            messageWriter.AssertWasCalled(mw => mw.Write("##teamcity[testIgnored name='my scenario' message='my suggestion']"));
        }

        private Scenario GetScenario(string name)
        {
            return new Scenario(name, null);
        }

        private Story GetStory(string summary)
        {
            return new Story(null, summary, null);
        }
    }
}
