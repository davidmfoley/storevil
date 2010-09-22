using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Core;

namespace StorEvil.Resharper.Elements
{
    public class StorEvilScenarioElement : StorEvilUnitTestElement, IStorEvilScenarioElement
    {
        private readonly UnitTestNamespace _namespace;
        public readonly IScenario Scenario;

        public StorEvilScenarioElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,
                                       string title, Scenario scenario)
            : base(provider, parent, project, title)
        {
            _namespace = new UnitTestNamespace(project.Name);
            Scenario = scenario;
        }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as StorEvilScenarioElement);
        }

        public override string GetTypeClrName()
        {
            return Scenario.Id;
        }

        public bool Equals(StorEvilScenarioElement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (!Scenario.Id.Equals(other.Scenario.Id))
                return false;

            return other.Scenario.Location.Path == Scenario.Location.Path && other.Scenario.Name == Scenario.Name;
            
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (_namespace != null ? _namespace.GetHashCode() : 0);
                result = (result*397) ^ (Scenario != null ? Scenario.Id.GetHashCode() : 0);
                return result;
            }
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            return DispositionBuilder.BuildDisposition(this, Scenario.Location, GetProjectFile(Scenario.Location.Path));
        }

        public string Id
        {
            get { return Scenario.Id; }
        }
    }

    public interface IStorEvilScenarioElement
    {
        string Id { get; }
    }
}