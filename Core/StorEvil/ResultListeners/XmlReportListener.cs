using System;
using System.Collections.Generic;
using StorEvil.Core;
using StorEvil.ResultListeners;

namespace StorEvil.InPlace
{
    public class XmlReportListener : IResultListener
    {
        private readonly IFileWriter _fileWriter;

        public XmlReportListener(IFileWriter fileWriter)
        {
            _fileWriter = fileWriter;
        }

        public void StoryStarting(Story story)
        {
            if (_currentStoryStatus != null)
                WriteCurrentStory();

            _currentStoryStatus = "Success";
            _scenarios = new List<string>();
        }

        private void WriteCurrentStory()
        {
            _stories.Add("<Story Id=\"id\" Summary=\"summary\" Status=\"" + _currentStoryStatus + "\">" + string.Join("\r\n", _scenarios.ToArray()) + "</Story>");

        }

        public void ScenarioStarting(Scenario scenario)
        {
            _currentScenario = "";
        }

        public void ScenarioFailed(Scenario scenario, string successPart, string failedPart, string message)
        {
            _currentScenario += LineMarkup(successPart, "Success");
            _currentScenario += LineMarkup(failedPart, "Failure");

            AddScenarioMarkup("Failure");
            _currentStoryStatus = "Failure";
        }

        public void CouldNotInterpret(Scenario scenario, string line)
        {
            throw new NotImplementedException();
        }

        public void Success(Scenario scenario, string line)
        {
            _currentScenario += LineMarkup(line, "Success");

        }

        private string LineMarkup(string line, string status)
        {
            return "<Line Status=\"" + status + "\">" + line + "</Line>\r\n";
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            AddScenarioMarkup("Success");
            
        }

        private void AddScenarioMarkup(string status)
        {
            _scenarios.Add("<Scenario Status=\"" + status + "\">" + _currentScenario+ "</Scenario>");
            _currentScenario = "";
        }

        private string _currentScenario = "";
        private string _currentStoryStatus;
        private List<string> _scenarios = new List<string>();
        private List<string> _stories = new List<string>();

        public void Finished()
        {
            WriteCurrentStory();
            _fileWriter.Write("<TestResults>" + string.Join("\r\n", _stories.ToArray())  + "</TestResults>");
               
        }
    }
}