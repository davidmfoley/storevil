using System;
using System.Collections.Generic;
using System.Linq;

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

        public ScenarioLine[] Background
        {
            get { return Scenario.Background; }            
        }

        public ScenarioLocation Location
        {
            get
            {
                return Scenario.Location;
            }
        }

        public IEnumerable<Scenario> Preprocess()
        {
            var scenario = Scenario;
            var count = 0;
            foreach (var example in Examples)
            {
                yield return
                    new Scenario(Location.Path, Id + " - " + (count++), scenario.Name,
                                 PreprocessLines(scenario.Body, FieldNames, example).ToArray())
                    {
                        Background = scenario.Background
                    };
            }
        }

        public ScenarioLine[] Body
        {
            get { return Scenario.Body; }
            
        }

        private IEnumerable<ScenarioLine> PreprocessLines(IEnumerable<ScenarioLine> lines, IEnumerable<string> fieldNames,
                                                    IEnumerable<string> example)
        {
            foreach (var line in lines)
            {
                var processed = line.Text;

                for (var fieldIndex = 0; fieldIndex < fieldNames.ToArray().Length; fieldIndex++)
                {
                    var name = fieldNames.ToArray()[fieldIndex];
                    processed = processed.Replace("<" + name + ">", example.ElementAtOrDefault(fieldIndex));
                }
                yield return new ScenarioLine { Text = processed, LineNumber = line.LineNumber, StartPosition = line.StartPosition };
            }
        }
    }
}