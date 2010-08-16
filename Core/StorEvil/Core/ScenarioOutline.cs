using System;

namespace StorEvil.Core
{
    [Serializable]
    public class ScenarioOutline : ScenarioBase, IScenario
    {
        public ScenarioOutline()
        {
        }

        public ScenarioOutline(string id, string name, Scenario scenario, string[] fieldNames,
                               string[][] examples)
        {
            Id = id;
            Name = name;
            Scenario = scenario;
            FieldNames = fieldNames;
            Examples = examples;
        }

        public Scenario Scenario { get; set; }
        public string[][] Examples { get; set; }

        public string[] FieldNames { get; set; }

        public ScenarioLocation Location
        {
            get
            {
                return Scenario.Location;
            }
        }
    }
}