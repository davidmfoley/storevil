using System;
using System.Collections.Generic;
using StorEvil.Core;

namespace StorEvil.Interpreter
{
    public class InterpreterForTypeFactory
    {
        private readonly ExtensionMethodHandler _extensionMethodHandler;
        static readonly Dictionary<Type, ScenarioInterpreterForType> _interpreterCache = new Dictionary<Type, ScenarioInterpreterForType>();

        public InterpreterForTypeFactory(ExtensionMethodHandler extensionMethodHandler)
        {
            _extensionMethodHandler = extensionMethodHandler;
        }

        public ScenarioInterpreterForType GetInterpreterForType(Type t)
        {
            if (!_interpreterCache.ContainsKey(t))
                _interpreterCache[t] = new ScenarioInterpreterForType(t, _extensionMethodHandler, this);

            return _interpreterCache[t];
        }
    }
}