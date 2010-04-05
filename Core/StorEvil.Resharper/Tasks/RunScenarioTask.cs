using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Core;

namespace StorEvil.Resharper
{
    [Serializable]
    public class RunScenarioTask : RemoteTask, IEquatable<RunScenarioTask>
    {
        public bool Explicitly { get; set; }
        private const string MagicDelimiter = "$*$*$";

        private readonly IEnumerable<ScenarioLine> Body;
        private readonly string Name;
        private bool IsOutline;
        private IEnumerable<string> FieldNames;
        private IEnumerable<IEnumerable<string>> Examples;

        public RunScenarioTask(IScenario scenario, bool explicitly)
            : base("StorEvil")
        {
            Logger.Log("RunScenarioTask - constructed\r\n" + scenario + "\r\n Explicitly:" + explicitly);
            Explicitly = explicitly;
            Id = scenario.Id;

            Name = scenario.Name;
            if (scenario is Scenario)
            {
                var s = scenario as Scenario;
                Body = s.Body;
            }
            else
            {
                var so = scenario as ScenarioOutline;
                Body = so.Scenario.Body;
                Examples = so.Examples;
                FieldNames = so.FieldNames;
            }
        }

        public RunScenarioTask(XmlElement element) : base(element)
        {
            var type = GetXmlAttribute(element, "ScenarioType");
            Name = GetXmlAttribute(element, "Name");
            Id = GetXmlAttribute(element, "Id");
            Body = GetXmlBody(element);

            if (type == "Scenario")
                LoadScenarioXml(element);
            else
                LoadScenarioOutlineXml(element);

            Logger.Log("RunScenarioTask - constructed from XML\r\n" + element.OuterXml);
        }

        private void LoadScenarioXml(XmlElement element)
        {
            IsOutline = false;
        }

        private void LoadScenarioOutlineXml(XmlElement element)
        {
            FieldNames = SplitValues(GetXmlAttribute(element, "FieldNames"));
            
            var exampleLines = GetXmlAttribute(element, "Examples").Split(new[] {"|||"}, StringSplitOptions.None);
            var examples = new List<IEnumerable<String>>();

            foreach (var exampleLine in exampleLines)
                examples.Add(SplitValues(exampleLine));

            Examples = examples;
            IsOutline = true;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "Name", Name);
            SetXmlAttribute(element, "Id", Id);
            SetXmlBody(element, Body);
            if (IsOutline)
                SaveScenarioOutlineXml(element);
            else
                SaveScenarioXml(element);
        }

        private void SaveScenarioXml(XmlElement element)
        {
            SetXmlAttribute(element, "ScenarioType", "Scenario");
        }

        private void SaveScenarioOutlineXml(XmlElement element)
        {
            SetXmlAttribute(element, "ScenarioType", "ScenarioOutline");
            SetXmlAttribute(element, "FieldNames", JoinValues(FieldNames));

            var examplesJoined = new List<string>();
            foreach (var example in Examples)
                examplesJoined.Add(JoinValues(example));

            var exampleValue = string.Join("|||", examplesJoined.ToArray());
            SetXmlAttribute(element, "Examples", exampleValue);
        }

        private IEnumerable<ScenarioLine> GetXmlBody(XmlElement element)
        {
            List<ScenarioLine> lines = new List<ScenarioLine>();

            var bodyElement = (XmlElement) element.GetElementsByTagName("Body")[0];
            var lineElements = bodyElement.GetElementsByTagName("Line");

            foreach (XmlElement lineElement in lineElements)
            {
                lines.Add(new ScenarioLine {Text = lineElement.GetAttribute("Text"), LineNumber = int.Parse(lineElement.GetAttribute("LineNumber"))});
            }

            return lines;
        }

        private IEnumerable<string> SplitValues(string text)
        {
            return text.Split(new[] {MagicDelimiter}, StringSplitOptions.None);
        }

        private void SetXmlBody(XmlElement element, IEnumerable<ScenarioLine> body)
        {
            var bodyElement = element.OwnerDocument.CreateElement("Body");
            element.AppendChild(bodyElement);
            foreach (var line in body)
            {
                var lineElement = element.OwnerDocument.CreateElement("Line");

                SetXmlAttribute(lineElement, "LineNumber", line.LineNumber.ToString());
                SetXmlAttribute(lineElement, "Text", line.Text);
                bodyElement.AppendChild(lineElement);
            }          
        }

        private string JoinValues(IEnumerable<string> values)
        {
            return string.Join("$*$*$", new List<string>(values).ToArray());
        }

        public IScenario GetScenario()
        {
            if (IsOutline)
            {
                return new ScenarioOutline(Id, Name, new Scenario(Name, Body), FieldNames, Examples);
            }
            else
            {
                return new Scenario(Id, Name, Body);
            }
        }

        public bool Equals(RunScenarioTask other)
        {
            var equals = other != null && other.Id == Id && other.Explicitly == Explicitly;
            return equals;
        }

        public override bool Equals(RemoteTask other)
        {
            if (other is RunScenarioTask)
            {
                var runScenarioTask = (RunScenarioTask)other;
                return runScenarioTask.Id == Id && runScenarioTask.Explicitly == this.Explicitly;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as RunScenarioTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ IsOutline.GetHashCode();
                result = (result*397) ^ Explicitly.GetHashCode();
                result = (result*397) ^ (Id != null ? Id.GetHashCode() : 0);
                return result;
            }
        }

        protected string Id { get; set; }

        public override string ToString()
        {
            return "RunScenarioTask \r\n Id=" + (Id ?? "") + "\r\n Name=" + (Name ?? "");
        }
    }
}