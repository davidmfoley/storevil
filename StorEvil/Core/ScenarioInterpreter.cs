using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Core
{
    public class ScenarioInterpreter
    {
        private readonly InterpreterForTypeFactory _interpreterFactory;

        public ScenarioInterpreter(InterpreterForTypeFactory interpreterFactory)
        {

            _interpreterFactory = interpreterFactory;
        }

        public InvocationChain GetChain(ScenarioContext storyContext, string line)
        {
           

            return GetSelectedChain(storyContext, line);
           
        }

      

        

        private InvocationChain GetSelectedChain(ScenarioContext storyContext, string line)
        {
            InvocationChain selectedChain = null;
            foreach (var type in storyContext.ImplementingTypes)
            {
                var interpreter = _interpreterFactory.GetInterpreterForType(type);

                InvocationChain chain = interpreter.GetChain(line);
                if (chain != null)
                {
                    selectedChain = chain;
                    break;
                }
            }
            return selectedChain;
        }
    }

   
}