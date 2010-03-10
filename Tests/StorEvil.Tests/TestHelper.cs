using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CSharp;
using NUnit.Framework;
using System.Collections.Generic;
using StorEvil.Utility;

namespace StorEvil
{
    public class TestHelper
    {
        public static Assembly CreateAssembly(string sourceCode)
        {
            Debug.Write(sourceCode);

            //ICodeCompiler compiler = 
            //add compiler parameters

            var compilerParams = new CompilerParameters
                                     {
                                         CompilerOptions = "/target:library",
                                         GenerateExecutable = false,
                                         GenerateInMemory = true,
                                         IncludeDebugInformation = false
                                     };


            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("nunit.framework.dll");

            compilerParams.ReferencedAssemblies.Add(typeof(TestHelper).Assembly.GetName().Name + ".dll");
            compilerParams.ReferencedAssemblies.Add(typeof(TestExtensionMethods).Assembly.GetName().Name + ".dll");
            
            var options = new Dictionary<string, string> {{"CompilerVersion", "v3.5"}};

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

    static class ExtensionMethods
    {
        public static IEnumerable<MethodInfo> GetTestMethods(this Type t)
        {
            foreach (var methodInfo in t.GetMethods())
            {
                if (methodInfo.DeclaringType == t)
                {
                    foreach (var attribute in methodInfo.GetCustomAttributes(false))
                    {
                        if (attribute is TestAttribute)
                        {
                            yield return methodInfo;
                            continue;
                        }
                    }

                }
            }

        }
    }
}