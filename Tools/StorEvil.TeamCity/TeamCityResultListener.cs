using System;
using System.Text.RegularExpressions;
using StorEvil.Core;
using StorEvil.Events;

namespace StorEvil.TeamCity
{
    public class TeamCityResultListener :
        IHandle<StoryStarting>,
        IHandle<ScenarioStarting>,
        IHandle<ScenarioFinished>,
        IHandle<StoryFinished>,
        IHandle<LineExecuted>
    {
        private const string TeamCityMessagePattern = "##teamcity[{0}]";

        private const string TestSuiteStartedMessagePattern = "testSuiteStarted name='{0}'";
        private const string TestSuiteFinishedMessagePattern = "testSuiteFinished name='{0}'";

        private const string TestStartedMessagePattern = "testStarted name='{0}'";
        private const string TestFailedMessagePattern = "testFailed name='{0}' message='{1}' details='{1}'";
        private const string TestFinishedMessagePattern = "testFinished name='{0}'";
        private const string TestIgnoredMessagePattern = "testIgnored name='{0}' message='{1}'";

        public void Handle(StoryStarting eventToHandle)
        {
            var summary = ExtractSummaryText(eventToHandle.Story);
            var message = String.Format(TestSuiteStartedMessagePattern, summary);
            SendTeamCityServiceMessage(message);
        }

        public void Handle(ScenarioStarting eventToHandle)
        {
            var message = String.Format(TestStartedMessagePattern, eventToHandle.Scenario.Name);
            SendTeamCityServiceMessage(message);
        }

        public void Handle(ScenarioFinished e)
        {
            if (e.Status == ExecutionStatus.Passed)
            {
                var message = String.Format(TestFinishedMessagePattern, e.Scenario.Name);
                SendTeamCityServiceMessage(message);
            }
        }

        public void Handle(StoryFinished eventToHandle)
        {
            var summary = ExtractSummaryText(eventToHandle.Story);
            var message = String.Format(TestSuiteFinishedMessagePattern, summary);
            SendTeamCityServiceMessage(message);
        }

        public void Handle(LineExecuted e)
        {
            if (e.Status == ExecutionStatus.Failed)
            {
                var message = String.Format(TestFailedMessagePattern, e.Scenario.Name, e.Message);
                SendTeamCityServiceMessage(message);
            }
            else if (e.Status == ExecutionStatus.Pending)
            {
                var message = String.Format(TestIgnoredMessagePattern, e.Scenario.Name,e.Suggestion);
                SendTeamCityServiceMessage(message);
            }
        }

        private void SendTeamCityServiceMessage(string message)
        {
            var teamCityMessage = String.Format(TeamCityMessagePattern, message);
            System.Console.WriteLine(teamCityMessage);
        }

        private string ExtractSummaryText(Story story)
        {
            var summary = Regex.Replace(story.Summary, @"feature\s*:\s*", String.Empty, RegexOptions.IgnoreCase);
            return StripNewlines(summary);
        }

        private string StripNewlines(string summary)
        {
            return summary
                .Replace("\r\n", " ")
                .Replace("\n", " ");
        }
    }

}
