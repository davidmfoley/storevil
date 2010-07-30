using System;
using System.Collections.Generic;

namespace StorEvil.Core
{
    [Serializable]
    public class Scenario : ScenarioBase, IScenario
    {
        public Scenario()
        {
        }

        public Scenario(string id, string name, ScenarioLine[] body)
        {
            Id = id;
            Name = name;
            Body = body;
        }

        public Scenario(string name, ScenarioLine[] body)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Body = body;
        }

        public ScenarioLine[] Body { get; set; }

       
    }

    [Serializable]
    public class ScenarioLine
    {
        public string Text { get; set; }

        public int LineNumber { get; set; }
    }

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
    }

    [Serializable]
    public class ScenarioBase
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public ScenarioLine[] Background { get; set; }
    }

    public interface IScenario
    {
        string Name { get; }
        string Id { get; }
        IEnumerable<string> Tags { get; }
        ScenarioLine[] Background { get; set; }
    }
}