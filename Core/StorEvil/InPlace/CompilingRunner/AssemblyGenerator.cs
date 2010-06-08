using System.Collections.Generic;
using System.IO;
using StorEvil.Core;
using StorEvil.InPlace.CompilingRunner;

namespace StorEvil.InPlace
{
    public class AssemblyGenerator
    {
        public AssemblyGenerator()
        {
            _handlerGenerator = new HandlerCodeGenerator();
            _compiler = new CodeCompiler();
        }
      
        private readonly HandlerCodeGenerator _handlerGenerator;
        private readonly CodeCompiler _compiler;

        public string GenerateAssembly(Story story, IEnumerable<Scenario> scenarios, IEnumerable<string> referencedAssemblies)
        {
            var sourceCode = _handlerGenerator.GetSourceCode(story, scenarios, referencedAssemblies);
            return _compiler.CompileToFile(sourceCode, referencedAssemblies, GetAssemblyLocation());
        }

        private static string GetAssemblyLocation()
        {
            return Path.GetTempFileName();
        }
    }
}