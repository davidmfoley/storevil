using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using StorEvil.Core;
using StorEvil.Nunit;

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
    {0}

    public class StorEvilDriver {{
        private readonly IResultListener _listener;
        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly ScenarioLineExecuter _lineExecuter;
        private readonly Story _story;
        private readonly IScenario[] _scenarios;

        public StorEvilDriver(  IResultListener listener, 
                                MemberInvoker memberInvoker,
                                ScenarioInterpreter scenarioInterpreter,
                                Story story)
        {{
            _listener = listener;
            _scenarioInterpreter = scenarioInterpreter;
            _lineExecuter = new ScenarioLineExecuter(memberInvoker, scenarioInterpreter, listener);
            _story = story;
            _scenarios = _story.Scenarios.ToArray();
        }}

        public bool Execute() {{
            int failures = 0;
            bool scenarioFailed;
            IScenario scenario;

            {1}

            return failures;
        }}
    }}
}}";
        public Assembly GenerateAssembly(Story story)
        {
            var codeBuilder = new StringBuilder();
            
            codeBuilder.AppendLine("// " + story.Id);
            AppendStoryCode(codeBuilder, story);
                
            var sourceCode = string.Format(_sourceCodeTemplate, "", codeBuilder.ToString());
            Debug.WriteLine(sourceCode);
            return CompileSource(sourceCode);
        }

        private void AppendStoryCode(StringBuilder codeBuilder, Story story)
        {
            var i = 0;
            foreach (var scenario in story.Scenarios)
            {
                codeBuilder.AppendLine(@"
scenario = _scenarios[" + i + @"];
scenarioFailed = false;");
                foreach (var line in GetLines(scenario))
                {
                    codeBuilder.Append(@"
if (!scenarioFailed) {
#line " + line.LineNumber + @" """ + story.Id + @"""
scenarioFailed &= _lineExecuter.ExecuteLine(scenario, null, @""" + line.Text.Replace("\"", "\"\"") + @""");
#line hidden
}    
");
                }
                codeBuilder.AppendLine(@"if (scenarioFailed) {failures++;}");

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