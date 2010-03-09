using System.Xml;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public class XmlReportListener : IResultListener
    {
        public class StatusNames
        {
            public const string Success = "Passed";
            public const string Failure = "Failed";
            public const string NotUnderstood = "NotUnderstood";
        }

        public class XmlNames
        {
            public const string Id = "Id";
            public const string Summary = "Summary";
            public const string Status = "Status";

            public const string Story = "Story";
            public const string Scenario = "Scenario";
            public const string Line = "Line";
            public const string DocumentElement = "TestResults";
        }

        private readonly IFileWriter _fileWriter;

        private readonly XmlDocument _doc;
        private XmlElement _currentStoryElement;
        private XmlElement _currentScenarioElement;

        public XmlReportListener(IFileWriter fileWriter)
        {
            _fileWriter = fileWriter;
            _doc = new XmlDocument();
            _doc.LoadXml("<" + XmlNames.DocumentElement + "/>");
        }

        public void StoryStarting(Story story)
        {
            _currentStoryElement = _doc.CreateElement(XmlNames.Story);
            _currentStoryElement.SetAttribute(XmlNames.Id, story.Id);
            _currentStoryElement.SetAttribute(XmlNames.Summary, story.Summary);
            SetStatus(_currentStoryElement, StatusNames.Success);
            _doc.DocumentElement.AppendChild(_currentStoryElement);
        }

        public void ScenarioStarting(Scenario scenario)
        {
            _currentScenarioElement = _doc.CreateElement(XmlNames.Scenario);
            _currentStoryElement.AppendChild(_currentScenarioElement);
        }

        public void ScenarioFailed(Scenario scenario, string successPart, string failedPart, string message)
        {
            AddLineToCurrentScenario(successPart, StatusNames.Success);
            AddLineToCurrentScenario(failedPart, StatusNames.Failure);

            SetStatus(_currentScenarioElement, StatusNames.Failure);
            SetStatus(_currentStoryElement, StatusNames.Failure);
        }

        private void AddLineToCurrentScenario(string successPart, string success)
        {
            _currentScenarioElement.AppendChild(GetLineElement(successPart, success));
        }

        private XmlNode GetLineElement(string line, string status)
        {
            var el = _doc.CreateElement(XmlNames.Line);
            SetStatus(el, status);
            el.InnerText = line;
            return el;
        }

        private static void SetStatus(XmlElement element, string status)
        {
            element.SetAttribute(XmlNames.Status, status);
        }

        public void CouldNotInterpret(Scenario scenario, string line)
        {
            AddLineToCurrentScenario(line, StatusNames.NotUnderstood);
            SetStatus(_currentScenarioElement, StatusNames.NotUnderstood);
            SetStatus(_currentStoryElement, StatusNames.Failure);
        }

        public void Success(Scenario scenario, string line)
        {
            AddLineToCurrentScenario(line, StatusNames.Success);
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            SetStatus(_currentScenarioElement, StatusNames.Success);
        }

        public void Finished()
        {
            _fileWriter.Write(_doc.OuterXml);
        }
    }
}