using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CSharp;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    public class CodeCompiler
    {
        public string CompileToFile(string sourceCode, IEnumerable<string> referencedAssemblies, string assemblyLocation)
        {   
            var compilerParams = BuildCompilerParams(referencedAssemblies);
            compilerParams.GenerateInMemory = false;
            compilerParams.OutputAssembly = assemblyLocation;

            var results = Compile(compilerParams, sourceCode);

            ThrowAndLogCompilerErrors(sourceCode, results);

            return assemblyLocation;
        }

        public Assembly CompileInMemory(string sourceCode, IEnumerable<string> referencedAssemblies)
        {
            var compilerParams = BuildCompilerParams(referencedAssemblies);
            compilerParams.GenerateInMemory = true;

            var results = Compile(compilerParams, sourceCode);

            ThrowAndLogCompilerErrors(sourceCode, results);

            return results.CompiledAssembly;
        }

        private static void ThrowAndLogCompilerErrors(string sourceCode, CompilerResults results)
        {
            
            if (results.Errors.Count == 0)
                return;

            foreach (var error in results.Errors)
                Debug.WriteLine(error);

            Debug.WriteLine(sourceCode);            

            throw new ApplicationException("Could not compile");
        }

        private CompilerResults Compile(CompilerParameters compilerParams, string sourceCode)
        {
            var options = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };

            return new CSharpCodeProvider(options)
                .CompileAssemblyFromSource(
                    compilerParams,
                    sourceCode);
        }

        private CompilerParameters BuildCompilerParams(IEnumerable<string> referencedAssemblies)
        {
            var compilerParams = new CompilerParameters
                                     {
                                         CompilerOptions = "/target:library /debug",
                                         GenerateExecutable = false,
                                         
                                         IncludeDebugInformation = true,
                                         
                                     };

            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Core.dll");

            foreach (var referencedAssembly in referencedAssemblies)
            {
                compilerParams.ReferencedAssemblies.Add(referencedAssembly);
            }
            string coreLocation = typeof(Story).Assembly.Location;
            compilerParams.ReferencedAssemblies.Add(coreLocation);
            return compilerParams;
        }
    }
}