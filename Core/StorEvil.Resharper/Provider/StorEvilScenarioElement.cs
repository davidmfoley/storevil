using System;
using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Core;

namespace StorEvil.Resharper
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
            return base.Equals(other) && Equals(other._namespace, _namespace);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (_namespace != null ? _namespace.GetHashCode() : 0);
            }
        }
    }
}