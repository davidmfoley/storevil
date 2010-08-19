using System;
using System.Collections.Generic;
using StorEvil.Context;

namespace StorEvil.Interpreter
{
    public interface IInterpreterForTypeFactory
    {
        ScenarioInterpreterForType GetInterpreterForType(Type t);
    }

    public class InterpreterForTypeFactory : IInterpreterForTypeFactory
    {
        private readonly ExtensionMethodHandler _extensionMethodHandler;
        private readonly Dictionary<Type, ScenarioInterpreterForType> _interpreterCache = new Dictionary<Type, ScenarioInterpreterForType>();

        public InterpreterForTypeFactory(AssemblyRegistry assemblyRegistry)
        {
            _extensionMethodHandler = new ExtensionMethodHandler(assemblyRegistry);
        }

        public ScenarioInterpreterForType GetInterpreterForType(Type t)
        {
            if (!_interpreterCache.ContainsKey(t))
                _interpreterCache[t] = new ScenarioInterpreterForType(t, _extensionMethodHandler.GetExtensionMethodsFor(t), this);

            return _interpreterCache[t];
        }
    }
}