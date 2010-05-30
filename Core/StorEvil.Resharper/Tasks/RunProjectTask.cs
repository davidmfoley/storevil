using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace StorEvil.Resharper
{
    [Serializable]
    public class RunProjectTask : RemoteTask, IEquatable<RunProjectTask>
    {
        public string Project { get; set; }
        public bool Explicitly { get; set; }

        public RunProjectTask(XmlElement element) : base(element)
        {
            Project = element.GetAttribute("Project");
            var assembliesJoined = element.GetAttribute("Assemblies");
            Assemblies = new List<string>(assembliesJoined.Split(new[] { "$$$" }, StringSplitOptions.None));
           
        }

        public RunProjectTask(string project, IEnumerable<string> assemblies, bool explicitly) : base("StorEvil")
        {
            Project = project;
            Explicitly = explicitly;
            Assemblies = new List<string>(assemblies);
        }

        public List<string> Assemblies { get; private set; }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "Project", Project);
            SetXmlAttribute(element, "Assemblies", string.Join("$$$", Assemblies.ToArray()));
        }

        public bool Equals(RunProjectTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Project == this.Project;
        }

        public override bool Equals(RemoteTask other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as RunProjectTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (Project != null ? Project.GetHashCode() : 0);
                result = (result*397) ^ Explicitly.GetHashCode();
                return result;
            }
        }
    }
}