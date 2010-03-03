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
            if (obj is StorEvilScenarioElement)
            {
                var scenarioElement = (StorEvilScenarioElement)obj;
                return scenarioElement.Scenario.Id == Scenario.Id;
            }

            return false;
        }

        public override string GetTypeClrName()
        {
            return Scenario.Id;
        }
    }

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
            if (obj is StorEvilScenarioElement)
            {
                var testElement = (StorEvilScenarioOutlineElement)obj;
                return testElement.GetNamespace().NamespaceName == _namespace.NamespaceName &&
                       testElement.GetTitle() == GetTitle();
            }

            return false;
        }


        public override string GetTypeClrName()
        {
            return _scenarioOutline.Id;
           
        }
    }
}