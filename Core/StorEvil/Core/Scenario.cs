using System;
using System.Collections.Generic;

namespace StorEvil.Core
{
    [Serializable]
    public class Scenario : IScenario   
    {
        public Scenario() {}

        public Scenario(string id, string name, IEnumerable<string> body) 
        {
         
            Id = id;
            Name = name;
            Body = body;
          
        }

        public Scenario(string name, IEnumerable<string> body)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Body = body;
        }

        public string Name { get; set; }
        public IEnumerable<string> Body { get; set; }
        public string Id { get; set; }
    }

    [Serializable]
    public class ScenarioOutline : IScenario   
    {
        public ScenarioOutline()
        {
        }

        public ScenarioOutline(string id, string name, Scenario scenario, IEnumerable<string> fieldNames, IEnumerable<IEnumerable<string>> examples)
        {
            Id = id;
            Name = name;
            Scenario = scenario;
            FieldNames = fieldNames;
            Examples = examples;
        }

        public Scenario Scenario { get; set; }
        public IEnumerable<IEnumerable<string>> Examples { get; set; }

        public IEnumerable<string> FieldNames { get; set; }

        public string Name { get; set; }
        public string Id { get; set; }
    }

    public interface IScenario
    {
         string Name { get; }
        string Id { get; }
    }
}