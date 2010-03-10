using StorEvil.Context;

namespace StorEvil.Interpreter
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
            foreach (var type in storyContext.ImplementingTypes)
            {
                var interpreter = _interpreterFactory.GetInterpreterForType(type);

                InvocationChain chain = interpreter.GetChain(line);
                if (chain != null)
                    return chain;
            }
            return null;
        }
    }
}