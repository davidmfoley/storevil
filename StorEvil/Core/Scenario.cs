using System;
using System.Collections;
using System.Collections.Generic;

namespace StorEvil.Core
{
    [Serializable]
    public class Scenario : IScenario   
    {
        public Scenario() {}

        public Scenario(string name, IEnumerable<string> body) : this(name, body, new IEnumerable<string>[0])
        {
            
        }
        public Scenario(string name, IEnumerable<string> body, IEnumerable<IEnumerable<string>> examples) 
           
        {
            Name = name;
            Body = body;
          
        }

        public string Name { get; set; }
        public IEnumerable<string> Body { get; set; }
    }

    [Serializable]
    public class ScenarioOutline : IScenario   
    {
        public ScenarioOutline()
        {
        }

        public ScenarioOutline(string name, Scenario scenario, IEnumerable<string> fieldNames, IEnumerable<IEnumerable<string>> examples)
        {
            Name = name;
            Scenario = scenario;
            FieldNames = fieldNames;
            Examples = examples;
        }

        public Scenario Scenario { get; set; }
        public IEnumerable<IEnumerable<string>> Examples { get; set; }

        public IEnumerable<string> FieldNames { get; set; }

        public string Name { get; set; }
    }

    public interface IScenario
    {
         string Name { get; }
        
    }
}