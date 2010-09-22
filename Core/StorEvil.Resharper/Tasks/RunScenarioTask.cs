using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Core;
using StorEvil.Resharper.Runner;

//using System.Linq;

namespace StorEvil.Resharper.Tasks
{
    [Serializable]
    public class RunScenarioTask : RemoteTask, IEquatable<RunScenarioTask>
    {
        public bool Explicitly { get; set; }


        private TaskScenarioLine[] Background;
        private TaskScenarioLine[] Body;
        private readonly string Name;

        public RunScenarioTask(Scenario scenario, bool explicitly)
            : base(StorEvilTaskRunner.RunnerId)
        {
            Explicitly = explicitly;
            Id = scenario.Id;

            Name = scenario.Name;

            var s = scenario;
            Body = XmlHelper.ConvertLines(s.Body);
            Background = XmlHelper.ConvertLines(s.Background);
        }


        public RunScenarioTask(XmlElement element) : base(element)
        {
            var type = GetXmlAttribute(element, "ScenarioType");
            Name = GetXmlAttribute(element, "Name");
            Id = GetXmlAttribute(element, "Id");
            Body = XmlHelper.GetXmlLines(element, "Body");
            Background = XmlHelper.GetXmlLines(element, "Background");
        }


        public override void SaveXml(XmlElement element)
        {
            Logger.Log("RunScenarioTask - SaveXml");
            base.SaveXml(element);
            SetXmlAttribute(element, "Name", Name);
            SetXmlAttribute(element, "Id", Id);
            XmlHelper.SetXmlLines(element, Body, "Body");
            XmlHelper.SetXmlLines(element, Background, "Background");
            SetXmlAttribute(element, "ScenarioType", "Scenario");
        }


        internal IScenario GetScenario()
        {
            return new Scenario("", Id, Name, XmlHelper.GetScenarioLines(Body))
                       {
                           Background = XmlHelper.GetScenarioLines(Background)
                       };
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
                var runScenarioTask = (RunScenarioTask) other;
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

    [Serializable]
    public class TaskScenarioLine
    {
        public string Text;
        public int LineNumber;
    }
}