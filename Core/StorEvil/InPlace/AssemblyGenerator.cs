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
    {0}

    public class StorEvilDriver {{
        private readonly IResultListener _listener;
        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly ScenarioLineExecuter _lineExecuter;

        public StorEvilDriver(  IResultListener listener, 
                                MemberInvoker memberInvoker,
                                ScenarioInterpreter scenarioInterpreter)
        {{
            _listener = listener;
            _scenarioInterpreter = scenarioInterpreter;
            _lineExecuter = new ScenarioLineExecuter(memberInvoker, scenarioInterpreter, listener);
        }}
        public int Execute(IResultListener listener) {{
            int failures = 0;
            {1}; 

            return failures;
        }}
    }}
}}";
        public Assembly GenerateAssembly(IEnumerable<Story> stories)
        {
            var codeBuilder = new StringBuilder();
            foreach (var story in stories)
            {
                codeBuilder.AppendLine("// " + story.Id);
                AppendStoryCode(codeBuilder, story);
                
            }
            var sourceCode = string.Format(_sourceCodeTemplate, "", codeBuilder.ToString());
            Debug.WriteLine(sourceCode);
            return CompileSource(sourceCode);
        }

        private void AppendStoryCode(StringBuilder codeBuilder, Story story)
        {
            
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