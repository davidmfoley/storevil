using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Core;

namespace StorEvil.Resharper
{
    public class StorEvilScenarioOutlineElement : StorEvilUnitTestElement
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


        public override string GetTypeClrName()
        {
            return _scenarioOutline.Id;
           
        }

        public bool Equals(StorEvilScenarioOutlineElement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._namespace, _namespace) && Equals(other._scenarioOutline, _scenarioOutline);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (_namespace != null ? _namespace.GetHashCode() : 0);
                result = (result*397) ^ (_scenarioOutline != null ? _scenarioOutline.GetHashCode() : 0);
                return result;
            }
        }
    }
}