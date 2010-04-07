using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Core;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public class HandlerCodeGenerator
    {
        private readonly IScenarioPreprocessor _scenarioPreprocessor;

        public HandlerCodeGenerator(IScenarioPreprocessor scenarioPreprocessor)
        {
            _scenarioPreprocessor = scenarioPreprocessor;
        }

        public string GetSourceCode(Story story, IEnumerable<Scenario> scenarios, IEnumerable<string> referencedAssemblies)
        {
            var codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("// " + story.Id);
            AppendStoryCode(codeBuilder, story, scenarios.ToArray());

            return string.Format(_sourceCodeTemplate, "", BuildContextFactorySetup(referencedAssemblies), codeBuilder.ToString());
        }

        private string BuildContextFactorySetup(IEnumerable<string> referencedAssemblies)
        {
            var adds =
                referencedAssemblies.Select(a => string.Format("contextFactory.AddAssembly(@\"{0}\");", a));
            return string.Join("\r\n            ", adds.ToArray());
        }

        private void AppendStoryCode(StringBuilder codeBuilder, Story story, Scenario[] scenarios)
        {
            var i = 0;
            foreach (var scenario in scenarios)
            {
                codeBuilder.AppendLine(@"
scenario = scenarios[" + i + @"];
scenarioFailed = false;
scenarioInterpreter.NewScenario();
_listener.ScenarioStarting(scenario);
context = storyContext.GetScenarioContext();
lastStatus = LineStatus.Passed;

#line 1  """ + story.Id + @"""
#line hidden");
                foreach (var line in GetLines(scenario))
                {
                    codeBuilder.AppendFormat(@"
if (lastStatus == LineStatus.Passed) {{
#line {0} 
lastStatus = lineExecuter.ExecuteLine(scenario, context, @""{1}"");
#line hidden
}}  
", line.LineNumber, line.Text.Replace("\"", "\"\""));
                }
                codeBuilder.AppendLine(@"if (lastStatus == LineStatus.Failed) {Result.Failed++; } else if (lastStatus == LineStatus.Pending) {Result.Pending++; } else { Result.Succeeded++; _listener.ScenarioSucceeded(scenario);}");

                i++;
            }
        }

        private IEnumerable<ScenarioLine> GetLines(IScenario scenario)
        {
            if (scenario is Scenario)
                return ((Scenario)scenario).Body;
            else
                return ((ScenarioOutline)scenario).Scenario.Body;
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
        IResultListener _listener;
        public StorEvilDriver(IResultListener listener) {{
            _listener = listener;
        }}

        public override void HandleStory(Story story) {{
             
             ScenarioInterpreter scenarioInterpreter = new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()));
             ScenarioPreprocessor scenarioPreprocessor = new ScenarioPreprocessor();
             Scenario[] scenarios = story.Scenarios.SelectMany(s=>scenarioPreprocessor.Preprocess(s)).ToArray();
                       
             var lineExecuter = new ScenarioLineExecuter(new MemberInvoker(), scenarioInterpreter, _listener);

            var contextFactory = new StoryContextFactory();            
            {1}
            var storyContext = contextFactory.GetContextForStory(story);         

            int failures = 0;
            bool scenarioFailed;
            Scenario scenario;
            ScenarioContext context;
            LineStatus lastStatus;

            {2}

            return;
        }}
    }}
}}";

    }
}