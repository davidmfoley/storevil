using System;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Core;

namespace StorEvil.Parsing
{
    public class ScenarioPreprocessor : IScenarioPreprocessor
    {
        public IEnumerable<Scenario> Preprocess(IScenario scenario)
        {
            if (scenario is ScenarioOutline)
                return PreprocessExamples((ScenarioOutline) scenario).ToArray();

            if (scenario is Scenario)
                return new[] {(Scenario) scenario};

            throw new ArgumentOutOfRangeException("");
        }

        private IEnumerable<Scenario> PreprocessExamples(ScenarioOutline outline)
        {
            var scenario = outline.Scenario;
            var count = 0;
            foreach (var example in outline.Examples)
            {
                yield return
                    new Scenario(outline.Id + (count++), scenario.Name,
                                 PreprocessLines(scenario.Body, outline.FieldNames, example).ToArray());
            }
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
                yield return new ScenarioLine {Text = processed, LineNumber = line.LineNumber};
            }
        }
    }
}