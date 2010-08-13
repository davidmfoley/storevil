using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using StorEvil.Core;

namespace StorEvil.Resharper.Elements
{
    public class StorEvilScenarioElement : StorEvilUnitTestElement
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
            return base.Equals(other) && Equals(other._namespace, _namespace) && Equals(other.Scenario, Scenario);
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
            var projectFile = GetProjectFile();
            
            //TextRange range = new TextRange();
            //var location = new UnitTestElementLocation(projectFile, range, range);

            var unitTestElementLocations = new UnitTestElementLocation[] {}; //new UnitTestElementLocation(), .Id};
            return new UnitTestElementDisposition(unitTestElementLocations, this);
        }

        private IProjectFile GetProjectFile()
        {
            return null;
            // var item = Project.ParentFolder.FindProjectItemByLocation(this.Scenario.Location.Path)
        }
    }
}