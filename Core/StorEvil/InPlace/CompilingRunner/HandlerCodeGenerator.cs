using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Core;

namespace StorEvil.InPlace.CompilingRunner
{
    public class HandlerCodeGenerator
    {
        public string GetSourceCode(Story story, IEnumerable<Scenario> scenarios,
                                    IEnumerable<string> referencedAssemblies)
        {
            var codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("// " + story.Id);
            AppendStoryCode(codeBuilder, story, scenarios.ToArray());

            return BuildSourceCode(codeBuilder, referencedAssemblies);
        }

        private string BuildSourceCode(StringBuilder codeBuilder, IEnumerable<string> referencedAssemblies)
        {
            return string.Format(_sourceCodeTemplate, "", BuildContextFactorySetup(referencedAssemblies), codeBuilder);
        }

        private string BuildContextFactorySetup(IEnumerable<string> referencedAssemblies)
        {
            var adds =
                referencedAssemblies.Select(a => string.Format("AddAssembly(@\"{0}\");", a));
            return string.Join("\r\n            ", adds.ToArray());
        }

        private void AppendStoryCode(StringBuilder codeBuilder, Story story, Scenario[] scenarios)
        {
            var i = 0;
            foreach (var scenario in scenarios)
            {
                codeBuilder.AppendLine(GetScenarioFunctionBody(i, story, scenario));

                i++;
            }
        }

        private string GetScenarioFunctionBody(int i, Story story, Scenario scenario)
        {
            var scenarioCodeBuilder = new StringBuilder();

            scenarioCodeBuilder.AppendLine(GetScenarioPreamble(i, story));

            foreach (var line in GetLines(scenario))
                scenarioCodeBuilder.AppendLine(BuildLine(line));

            scenarioCodeBuilder.AppendLine(GetScenarioEnd());

            return scenarioCodeBuilder.ToString();
        }

        private string GetScenarioEnd()
        {
            return "}\r\nCollectScenarioResult();";
        }

        private string GetScenarioPreamble(int i, Story story)
        {
            return @"
scenario = scenarios[" + i + @"];

using (StartScenario(story, scenario)) {

#line 1  """ +
                   story.Id + @"""
#line hidden";
        }

        private string BuildLine(ScenarioLine line)
        {
            var lineCodeBuilder = new StringBuilder();

            lineCodeBuilder.AppendFormat(
                @"
if (ShouldContinue) {{
#line {0} 
StorEvilContexts = ExecuteLine(@""{1}"");
#line hidden
}}  
", line.LineNumber,
                line.Text.Replace("\"", "\"\""));
            return lineCodeBuilder.ToString();
        }

        private IEnumerable<ScenarioLine> GetLines(IScenario scenario)
        {
            if (scenario is Scenario)
            {
                var s = (Scenario) scenario;
                return (s.Background ?? new ScenarioLine[0]).Union( s.Body);
            }
            else
            {
                var s = ((ScenarioOutline)scenario).Scenario;
                return (s.Background ?? new ScenarioLine[0]).Union(s.Body);
            }
        }

        private string _sourceCodeTemplate =
            @"
namespace StorEvilTestAssembly {{
    using StorEvil.Context;
    using StorEvil.Core;
    using StorEvil.Interpreter;
    using StorEvil.InPlace;
    using System.Linq;
    using StorEvil.Parsing;
    using StorEvil.ResultListeners;
    {0}

    public class StorEvilDriver : StorEvil.InPlace.DriverBase  {{        
        public StorEvilDriver(IResultListener listener) : base(listener) {{
           
        }}

        public override void HandleStory(Story story) {{
                                    
            {1}
            var scenarios = GetScenarios(story);
            var StorEvilContexts = new object[0];
            Scenario scenario;

            {2}

            return;
        }}
    }}
}}";
    }
}