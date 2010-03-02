using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Text;
using StorEvil.Core;

namespace StorEvil.Resharper
{
    public abstract class StorEvilUnitTestElement : UnitTestElement
    {
        protected readonly IProject Project;
        private readonly string _title;

        protected StorEvilUnitTestElement(IUnitTestProvider provider, UnitTestElement parent, IProject project,
                                       string title) : base(provider, parent)
        {
            Project = project;
            _title = title;
        }

        public override IProject GetProject()
        {
            return Project;
        }

        public override string GetTitle()
        {
            return _title;
        }

        public override string GetTypeClrName()
        {
            return GetProject().Name + "." + _title;
        }

        public override IList<IProjectFile> GetProjectFiles()
        {
            return new List<IProjectFile>();
        }

        public override string GetKind()
        {
            return "StorEvil";
        }

        public override string ShortName
        {
            get { return _title; }
        }

        public override bool Matches(string filter, IdentifierMatcher matcher)
        {
            return true;
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = GetDeclaredElement();
            if (element != null && element.IsValid())
            {
                var locations = new List<UnitTestElementLocation>();
                foreach (IDeclaration declaration in element.GetDeclarations())
                {
                    IFile file = declaration.GetContainingFile();
                    if (file != null)
                        locations.Add(new UnitTestElementLocation(file.ProjectFile,
                                                                  declaration.GetDocumentRange().TextRange,
                                                                  declaration.GetDocumentRange().TextRange));
                }
                return new UnitTestElementDisposition(locations, this);
            }
            return UnitTestElementDisposition.InvalidDisposition;
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return null;
        }

        protected ITypeElement GetDeclaredType()
        {
            return null;
        }

        public virtual IList<UnitTestTask> GetTaskSequence()
        {
            return new List<UnitTestTask>();
        }
    }

    public class StorEvilScenarioElement : StorEvilUnitTestElement
    {
        private readonly UnitTestNamespace _namespace;
        private readonly IScenario _scenario;

        public StorEvilScenarioElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,
                                       string title, IScenario scenario)
            : base(provider, parent, project, title)
        {
            _namespace = new UnitTestNamespace(project.Name);
            _scenario = scenario;
        }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (obj is StorEvilScenarioElement)
            {
                var testElement = (StorEvilUnitTestElement) obj;
                return testElement.GetNamespace().NamespaceName == _namespace.NamespaceName &&
                       testElement.GetTitle() == GetTitle();
            }

            return false;
        }

        public override IList<UnitTestTask> GetTaskSequence()
        {
            return new List<UnitTestTask>() { new UnitTestTask(this, new RunScenarioTask(_scenario))};
        }
    }

    public class RunScenarioTask : RemoteTask
    {
        public IScenario Scenario { get; set; }
        private readonly StorEvilScenarioElement _element;

        public RunScenarioTask()
            : base("StorEvil")
        {
        }

        public RunScenarioTask(XmlElement element) : base(element)
        {
            
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
          
        }

        public RunScenarioTask(IScenario scenario) : base("StorEvil")
        {
            Scenario = scenario;
        }
    }

    public class StorEvilProjectElement : StorEvilUnitTestElement
    {
        private readonly UnitTestNamespace _namespace = new UnitTestNamespace("namespace.foo");

        public StorEvilProjectElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,
                                       string title)
            : base(provider, parent, project, title)
        {
            _namespace = new UnitTestNamespace(project.Name + ".namespaceYo");
        }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (obj is StorEvilUnitTestElement)
            {
                var testElement = (StorEvilUnitTestElement)obj;
                return testElement.GetNamespace().NamespaceName == _namespace.NamespaceName &&
                       testElement.GetTitle() == GetTitle();
            }

            return false;
        }
    }

    public class StorEvilStoryElement : StorEvilUnitTestElement
    {
        private readonly UnitTestNamespace _namespace = new UnitTestNamespace("namespace.foo");

        public StorEvilStoryElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,
                                       string title)
            : base(provider, parent, project, title)
        {
            _namespace = new UnitTestNamespace(project.Name + ".namespaceYo");
        }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (obj is StorEvilUnitTestElement)
            {
                var testElement = (StorEvilUnitTestElement)obj;
                return testElement.GetNamespace().NamespaceName == _namespace.NamespaceName &&
                       testElement.GetTitle() == GetTitle();
            }

            return false;
        }
    }
}