using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context;
using StorEvil.Utility;

namespace StorEvil.Interpreter
{
  

    public class ScenarioInterpreter
    {
        private readonly IInterpreterForTypeFactory _interpreterFactory;
        private readonly IAmbiguousMatchResolver _resolver;
        private string _lastSignificantFirstWord = null;

        public ScenarioInterpreter(IInterpreterForTypeFactory interpreterFactory, IAmbiguousMatchResolver resolver )
        {
            _interpreterFactory = interpreterFactory;
            _resolver = resolver;
        }

        public InvocationChain GetChain(ScenarioContext context, string line)
        {
            DebugTrace.Trace("Interpreting", line);
            var chains = (GetSelectedChains(context, line) ?? new InvocationChain[0]).ToArray();
            if (! chains.Any())
                DebugTrace.Trace(this.GetType().Name, "no match: " + line);

            if (chains.Count() > 1)
                return _resolver.ResolveMatch(line, chains);

            return chains.FirstOrDefault();
        }

        public void NewScenario()
        {
            _lastSignificantFirstWord = null;
        }

        private IEnumerable<InvocationChain> GetSelectedChains(ScenarioContext context, string line)
        {
            foreach (var linePermutation in GetPermutations(line))
            {
                foreach (var type in context.ImplementingTypes)
                {
                    var interpreter = _interpreterFactory.GetInterpreterForType(type);

                    IEnumerable<InvocationChain> chains = interpreter.GetChains(linePermutation);
                    foreach (var invocationChain in chains)
                    {
                        yield return invocationChain;
                    }                    
                }
            }
           
        }

        private IEnumerable<string> GetPermutations(string line)
        { 
            if (line.ToLower().StartsWith("and "))
            {
                yield return line;
                yield return line.ReplaceFirstWord(_lastSignificantFirstWord);
            }
            else
            {
                _lastSignificantFirstWord = line.Split(' ').First().Trim();
                yield return line;                
            }
        }
    }
}