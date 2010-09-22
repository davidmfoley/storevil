using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Core;
using StorEvil.Resharper.Elements;

namespace StorEvil.Resharper
{
    public class StorEvilScenarioOutlineElement : StorEvilUnitTestElement, IStorEvilScenarioElement
    {
        private readonly ScenarioOutline _scenarioOutline;
        private readonly UnitTestNamespace _namespace;

        public StorEvilScenarioOutlineElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,
                                              string title, ScenarioOutline scenarioOutline)
            : base(provider, parent, project, title)
        {
            _scenarioOutline = scenarioOutline;
            _namespace = new UnitTestNamespace(project.Name);
        }

        public ScenarioOutline ScenarioOutline { get { return _scenarioOutline; } }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as StorEvilScenarioOutlineElement);
        }

        public string Id
        {
            get { return _scenarioOutline.Id; }
        }
        public override string GetTypeClrName()
        {
            return _scenarioOutline.Id;
           
        }

        public bool Equals(StorEvilScenarioOutlineElement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return (ScenarioOutline.Id.Equals(other.ScenarioOutline.Id));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (_scenarioOutline.Id != null ? _scenarioOutline.Id.GetHashCode() : 0);
                return result;
            }
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            return DispositionBuilder.BuildDisposition(this, ScenarioOutline.Location, GetProjectFile(ScenarioOutline.Location.Path));
        }
    }
}