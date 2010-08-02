using System;
using System.Xml;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Infrastructure;
using StorEvil.InPlace;

namespace StorEvil.ResultListeners
{
    public class XmlReportListener : IEventHandler<SessionFinishedEvent>,
        IEventHandler<ScenarioFailedEvent, ScenarioPendingEvent, LineInterpretedEvent>,
        IEventHandler<LineNotInterpretedEvent, ScenarioStartingEvent>
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

        private readonly ITextWriter _fileWriter;

        private readonly XmlDocument _doc;
        private XmlElement _currentStoryElement;
        private XmlElement _currentScenarioElement;

        public XmlReportListener(ITextWriter fileWriter)
        {
            _fileWriter = fileWriter;
            _doc = new XmlDocument();
            _doc.LoadXml("<" + XmlNames.DocumentElement + "/>");
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

        public void Handle(ScenarioSucceededEvent eventToHandle)
        {
            SetStatus(_currentScenarioElement, StatusNames.Success);
        }

        public void Handle(StoryStartingEvent eventToHandle)
        {
            var story = eventToHandle.Story;
            _currentStoryElement = _doc.CreateElement(XmlNames.Story);
            _currentStoryElement.SetAttribute(XmlNames.Id, story.Id);
            _currentStoryElement.SetAttribute(XmlNames.Summary, story.Summary);
            SetStatus(_currentStoryElement, StatusNames.Success);
            _doc.DocumentElement.AppendChild(_currentStoryElement);
        }

        public void Handle(SessionFinishedEvent eventToHandle)
        {
            _fileWriter.Write(_doc.OuterXml);
        }

        public void Handle(ScenarioFailedEvent eventToHandle)
        {
            AddLineToCurrentScenario(eventToHandle.SuccessPart, StatusNames.Success);
            AddLineToCurrentScenario(eventToHandle.FailedPart, StatusNames.Failure);

            SetStatus(_currentScenarioElement, StatusNames.Failure);
            SetStatus(_currentStoryElement, StatusNames.Failure);
        }

        public void Handle(ScenarioPendingEvent eventToHandle)
        {
            AddLineToCurrentScenario(eventToHandle.Line, StatusNames.NotUnderstood);
            SetStatus(_currentScenarioElement, StatusNames.NotUnderstood);
            SetStatus(_currentStoryElement, StatusNames.Failure);
        }

        public void Handle(LineInterpretedEvent eventToHandle)
        {
            AddLineToCurrentScenario(eventToHandle.Line, StatusNames.Success);
        }

        public void Handle(LineNotInterpretedEvent eventToHandle)
        {
            AddLineToCurrentScenario(eventToHandle.Line, StatusNames.NotUnderstood);
            SetStatus(_currentScenarioElement, StatusNames.NotUnderstood);
            SetStatus(_currentStoryElement, StatusNames.Failure);
        }

        public void Handle(ScenarioStartingEvent eventToHandle)
        {
            _currentScenarioElement = _doc.CreateElement(XmlNames.Scenario);
            _currentStoryElement.AppendChild(_currentScenarioElement);
        }
    }

    public class AutoRegisterForEvents
    {
        public AutoRegisterForEvents()
        {
            StorEvilEvents.Bus.Register(this);
        }
    }
}