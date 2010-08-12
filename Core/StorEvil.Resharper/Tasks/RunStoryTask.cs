using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Resharper.Runner;

namespace StorEvil.Resharper.Tasks
{
    [Serializable]
    public class LoadContextAssemblyTask : RemoteTask
    {
        public readonly string AssemblyPath;

        public LoadContextAssemblyTask(XmlElement element) : base(element)
        {
            AssemblyPath = GetXmlAttribute(element, "AssemblyPath");
        }

        public LoadContextAssemblyTask(string assemblyPath)
            : base(StorEvilTaskRunner.RunnerId)
        {
            AssemblyPath = assemblyPath;
        }

        public override void SaveXml(System.Xml.XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "AssemblyPath", AssemblyPath);

        }

        public bool Equals(LoadContextAssemblyTask other)
        {
            return other != null && other.AssemblyPath == AssemblyPath;
        }

        public override bool Equals(RemoteTask other)
        {
            if (other == null) return false;

            return ReferenceEquals(this, other) ||
                Equals(other as LoadContextAssemblyTask);
        }
    }
    [Serializable]
    public class RunStoryTask : RemoteTask
    {
        private readonly string _id;
        private readonly bool _explicitly;

        public RunStoryTask(XmlElement element) : base(element)
        {
            _id = GetXmlAttribute(element, "Id");
        }

        public RunStoryTask(string id, bool explicitly) : base(StorEvilTaskRunner.RunnerId)
        {
            _id = id;
            _explicitly = explicitly;
        }

        public override void SaveXml(System.Xml.XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "Id", _id);

        }

        public bool Equals(RunStoryTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return false;
        }

        public override bool Equals(RemoteTask other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as RunStoryTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (_id != null ? _id.GetHashCode() : 0);
                result = (result*397) ^ _explicitly.GetHashCode();
                return result;
            }
        }
    }
}