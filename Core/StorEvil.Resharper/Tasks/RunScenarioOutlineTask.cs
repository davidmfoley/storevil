using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Core;
using StorEvil.Resharper.Runner;

namespace StorEvil.Resharper.Tasks
{
    [Serializable]
    public class RunScenarioOutlineTask : RemoteTask, IEquatable<RunScenarioOutlineTask>
    {
        private string[] FieldNames;

        private string[][] Examples;
        public bool Explicitly { get; set; }
        private const string MagicDelimiter = "$*$*$";

        private TaskScenarioLine[] Background;
        private TaskScenarioLine[] Body;
        private readonly string Name;
        protected string Id { get; set; }

        public RunScenarioOutlineTask(ScenarioOutline scenario, bool explicitly)
            : base(StorEvilTaskRunner.RunnerId)
        {
            Logger.Log("RunScenarioTask - constructed\r\n" + scenario.Id + " - " + scenario.Name + "\r\n Explicitly:" + explicitly);
            Explicitly = explicitly;
            Id = scenario.Id;

            Name = scenario.Name;
           
            var so = scenario as ScenarioOutline;
            Body = XmlHelper.ConvertLines(so.Scenario.Body);
            Background = XmlHelper.ConvertLines(so.Scenario.Background);
            Examples = so.Examples;
            FieldNames = so.FieldNames;           
        }

        public RunScenarioOutlineTask(XmlElement element)
            : base(element)
        {
           
            Name = GetXmlAttribute(element, "Name");
            Id = GetXmlAttribute(element, "Id");
            Body = XmlHelper.GetXmlLines(element, "Body");
            Background = XmlHelper.GetXmlLines(element, "Background");
           
            LoadScenarioOutlineXml(element);            
        }

        private void LoadScenarioOutlineXml(XmlElement element)
        {
            FieldNames = XmlHelper.SplitValues(GetXmlAttribute(element, "FieldNames"));

            var exampleLines = GetXmlAttribute(element, "Examples").Split(new[] { "|||" }, StringSplitOptions.None);
            var examples = new List<string[]>();

            foreach (var exampleLine in exampleLines)
                examples.Add(XmlHelper.SplitValues(exampleLine));

            Examples = examples.ToArray();
           
        }

        public override void SaveXml(XmlElement element)
        {
            Logger.Log("RunScenarioTask - SaveXml");
            base.SaveXml(element);
            SetXmlAttribute(element, "Name", Name);
            SetXmlAttribute(element, "Id", Id);
            XmlHelper.SetXmlLines(element, Body, "Body");
            XmlHelper.SetXmlLines(element, Background, "Background");
            SaveScenarioOutlineXml(element);
            Logger.Log(element.OuterXml);
        }

        public bool Equals(RunScenarioOutlineTask other)
        {
            var equals = other != null && other.Id == Id && other.Explicitly == Explicitly;
            return equals;
        }

        public override bool Equals(RemoteTask other)
        {
            if (other is RunScenarioOutlineTask)
            {
                return Equals((RunScenarioOutlineTask) other);
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as RunScenarioOutlineTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ Explicitly.GetHashCode();
                result = (result * 397) ^ (Id != null ? Id.GetHashCode() : 0);
                return result;
            }
        }

        internal IScenario GetScenario()
        {            
            return new ScenarioOutline(Id, Name, new Scenario(Name,
                                                              XmlHelper.GetScenarioLines(Body)) { Background = XmlHelper.GetScenarioLines(Background) }, FieldNames, Examples) 
                                                              ;            
        }

        private void SaveScenarioOutlineXml(XmlElement element)
        {
            SetXmlAttribute(element, "ScenarioType", "ScenarioOutline");
            SetXmlAttribute(element, "FieldNames", XmlHelper.JoinValues(FieldNames));

            var examplesJoined = new List<string>();
            foreach (var example in Examples)
                examplesJoined.Add(XmlHelper.JoinValues(example));

            var exampleValue = string.Join("|||", examplesJoined.ToArray());
            SetXmlAttribute(element, "Examples", exampleValue);
        }    

        

        public bool HasChildWithId(string id)
        {
            return (id.StartsWith(Id));
        }

        public bool IsLastChild(string id)
        {
            if (!HasChildWithId(id))
                return false;

            var pieces = id.Split('-');
            var last = pieces[pieces.Length -1].Trim();
            return last == (Examples.Length - 1).ToString();
        }

        public bool IsFirstChild(string id)
        {
            if (!HasChildWithId(id))
                return false;

            var pieces = id.Split('-');
            var last = pieces[pieces.Length - 1].Trim();
            return last == "0";
        }
    }
}