using System.Collections.Generic;
using System.IO;
using StorEvil.Core;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public class AssemblyGenerator
    {
        public AssemblyGenerator(IScenarioPreprocessor scenarioPreprocessor)
        {
            _handlerGenerator = new HandlerCodeGenerator(scenarioPreprocessor);
            _compiler = new CodeCompiler();
        }
      
        private HandlerCodeGenerator _handlerGenerator;
        private CodeCompiler _compiler;

        public string GenerateAssembly(Story story, IEnumerable<Scenario> scenarios, IEnumerable<string> referencedAssemblies)
        {
            string sourceCode = _handlerGenerator.GetSourceCode(story, scenarios, referencedAssemblies);
            return _compiler.CompileToFile(sourceCode, referencedAssemblies, GetAssemblyLocation());
        }

        private static string GetAssemblyLocation()
        {
            return Path.GetTempFileName();
        }
    }
}