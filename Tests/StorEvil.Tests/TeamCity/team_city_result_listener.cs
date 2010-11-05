using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Assertions;

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

            WrittenMessagesShouldContain("##teamcity[testSuiteStarted name='my summary']");
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_story_execution_is_finished()
        {
            listener.Handle(new StoryFinished() { Story = GetStory("Feature   : my \n summary \n    ")});

            WrittenMessagesShouldContain("##teamcity[testSuiteFinished name='my summary']");
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_scenario_starts_being_executed()
        {
            listener.Handle(new ScenarioStarting() { Scenario = GetScenario("my scenario") });

            WrittenMessagesShouldContain("##teamcity[testStarted name='my scenario']");
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_scenario_execution_passed()
        {
            listener.Handle(new ScenarioPassed() { Scenario = GetScenario("my scenario") });

            WrittenMessagesShouldContain("##teamcity[testFinished name='my scenario']");
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_scenario_execution_failed()
        {
            listener.Handle(new LineFailed() { Scenario = GetScenario("my scenario"),ExceptionInfo = "error message [ ] ' \r" });

            WrittenMessagesShouldContain("##teamcity[testFailed name='my scenario' message='error message |[ |] |' |r' details='error message |[ |] |' |r']");
        }

        [Test]
        public void should_output_correct_team_city_message_when_a_scenario_execution_is_inconclusive()
        {
            listener.Handle(new LinePending() { Scenario = GetScenario("my scenario"), Suggestion = "my suggestion" });

            messageWriter.AssertWasCalled(mw => mw.Write("##teamcity[testIgnored name='my scenario' message='my suggestion']"));
        }

        private void WrittenMessagesShouldContain(string teamCityMessage)
        {
            var writtenMessages = messageWriter.GetArgumentsForCallsMadeOn(x => x.Write(Arg<string>.Is.Anything));
            writtenMessages.Select(x => x[0]).ShouldContain(teamCityMessage);
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
