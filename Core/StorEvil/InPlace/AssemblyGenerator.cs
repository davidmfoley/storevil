using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    public class AssemblyGenerator
    {
       

        private string _sourceCodeTemplate =
            @"
namespace StorEvilTestAssembly {{
    using StorEvil.Context;
    using StorEvil.Core;
    using StorEvil.Interpreter;
    using StorEvil.InPlace;
    using System.Linq;
    using StorEvil.Parsing;
    {0}

    public class StorEvilDriver {{
        private readonly IResultListener _listener;
        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly ScenarioLineExecuter _lineExecuter;
        private readonly Scenario[] _scenarios;
        private readonly ScenarioPreprocessor _scenarioPreprocessor;

        public StorEvilDriver(  IResultListener listener, 
                                MemberInvoker memberInvoker,
                                ScenarioInterpreter scenarioInterpreter,                            
                                Scenario[] scenarios)
        {{
            _listener = listener;
            _scenarioInterpreter = scenarioInterpreter;
            _lineExecuter = new ScenarioLineExecuter(memberInvoker, scenarioInterpreter, listener);
            
            _scenarios = scenarios;
        }}

        public int Execute(StoryContext storyContext) {{
            int failures = 0;
            bool scenarioFailed;
            Scenario scenario;
            ScenarioContext context;

            {1}

            return failures;
        }}
    }}
}}";
        public Assembly GenerateAssembly(Story story, Scenario[] scenarios)
        {
            var codeBuilder = new StringBuilder();
            
            codeBuilder.AppendLine("// " + story.Id);
            AppendStoryCode(codeBuilder, story, scenarios);
                
            var sourceCode = string.Format(_sourceCodeTemplate, "", codeBuilder.ToString());            
            return CompileSource(sourceCode);
        }

        private void AppendStoryCode(StringBuilder codeBuilder, Story story, Scenario[] scenarios)
        {
            var i = 0;
            foreach (var scenario in scenarios)
            {
                codeBuilder.AppendLine(@"
scenario = _scenarios[" + i + @"];
scenarioFailed = false;
_scenarioInterpreter.NewScenario();
_listener.ScenarioStarting(scenario);
context = storyContext.GetScenarioContext();");
                foreach (var line in GetLines(scenario))
                {
                    codeBuilder.AppendFormat(@"
if (!scenarioFailed) {{
#line {0} ""{1}""
scenarioFailed = scenarioFailed || !_lineExecuter.ExecuteLine(scenario, context, @""{2}"");
#line hidden
}}  
", line.LineNumber, story.Id, line.Text.Replace("\"", "\"\""));
                }
                codeBuilder.AppendLine(@"if (scenarioFailed) {failures++;} else { _listener.ScenarioSucceeded(scenario);}");

                i++;
            }
        }

        private IEnumerable<ScenarioLine> GetLines(IScenario scenario)
        {
            if (scenario is Scenario)
                return ((Scenario) scenario).Body;
            else
                return ((ScenarioOutline) scenario).Scenario.Body;
        }

        private Assembly CompileSource(string sourceCode)
        {
            Debug.Write(sourceCode);
            var compilerParams = new CompilerParameters
                                     {
                                         CompilerOptions = "/target:library",
                                         GenerateExecutable = false,
                                         GenerateInMemory = true,
                                         IncludeDebugInformation = true
                                     };

            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Core.dll");
            compilerParams.ReferencedAssemblies.Add(typeof(Story).Assembly.Location);

            var options = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };

            var results = new CSharpCodeProvider(options)
                .CompileAssemblyFromSource(
                    compilerParams,
                    sourceCode);

            if (results.Errors.Count != 0)
            {
                foreach (var error in results.Errors)
                    Debug.WriteLine(error);

                throw new ApplicationException("Could not compile");
            }

            //return the assembly
            return results.CompiledAssembly;
        }
    }
}