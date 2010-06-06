using System;
using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;

namespace StorEvil.Resharper
{
    public class StorEvilProjectElement : StorEvilUnitTestElement
    {
        private readonly UnitTestNamespace _namespace = new UnitTestNamespace("namespace.foo");

        public StorEvilProjectElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,
                                      string title, IEnumerable<string> assemblies)
            : base(provider, parent, project, title)
        {
            Assemblies = assemblies;
            _namespace = new UnitTestNamespace(project.Name + " StorEvil specs");
        }

        public IEnumerable<string> Assemblies { get; set; }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as StorEvilProjectElement);
        }

        public bool Equals(StorEvilProjectElement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._namespace, _namespace) && Equals(other.Assemblies, Assemblies);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (_namespace != null ? _namespace.GetHashCode() : 0);
                result = (result*397) ^ (Assemblies != null ? Assemblies.GetHashCode() : 0);
                return result;
            }
        }
    }

    [Serializable]
    public class ProjectTask : RemoteTask, IEquatable<ProjectTask>
    {
        public bool Explicitly { get; set; }

        public ProjectTask(StorEvilProjectElement storEvilProjectElement, bool explicitly) : base("StorEvil")
        {
            Explicitly = explicitly;
            Id = storEvilProjectElement.GetNamespace().NamespaceName;
        }

        protected string Id { get; set; }

        public bool Equals(ProjectTask other)
        {
            return Equals(other.Id, Id) && other.Explicitly == Explicitly;
        }

        public override bool Equals(RemoteTask other)
        {
            if (other is ProjectTask)
                return Equals((ProjectTask) other);

            return false;
        }
    }
}