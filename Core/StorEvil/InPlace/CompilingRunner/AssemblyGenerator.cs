using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

        private static string GetAssemblyLocation()
        {
            return Path.GetTempFileName();
        }

        public string GenerateAssembly(AssemblyGenerationSpec spec)
        {
            var sourceCode =_handlerGenerator.GetSourceCode(spec);

            return _compiler.CompileToFile(sourceCode, spec.Assemblies, GetAssemblyLocation());
        }

        public Assembly GenerateAssemblyInMemory(AssemblyGenerationSpec spec)
        {
            var sourceCode = _handlerGenerator.GetSourceCode(spec);

            return _compiler.CompileInMemory(sourceCode,  spec.Assemblies);
        }
    }
}