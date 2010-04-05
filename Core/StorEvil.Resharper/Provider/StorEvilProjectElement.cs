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
            _namespace = new UnitTestNamespace(project.Name + ".namespaceYo");
        }

        public IEnumerable<string> Assemblies { get; set; }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (obj is StorEvilProjectElement)
            {
                var testElement = (StorEvilProjectElement)obj;
                return testElement.GetNamespace().NamespaceName == _namespace.NamespaceName &&
                       testElement.GetTitle() == GetTitle();
            }

            return false;
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