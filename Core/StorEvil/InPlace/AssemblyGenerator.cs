using System;
using System.Collections.Generic;
using System.IO;
using StorEvil.Core;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{

    public abstract class DriverBase : MarshalByRefObject, IStoryHandler
    {
        protected JobResult Result = new JobResult();

        public abstract void HandleStory(Story story);

        public void Finished()
        {
           
        }

        public JobResult GetResult()
        {
            return Result;
        }
    }

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