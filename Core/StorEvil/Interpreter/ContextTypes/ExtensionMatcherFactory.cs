using System;
using System.Collections.Generic;
using StorEvil.Context.Matchers;

namespace StorEvil.Interpreter
{
    public class ExtensionMatcherFactory : IMatcherFactory
    {
        private readonly IExtensionMethodHandler _extensionMethodHandler;

        public ExtensionMatcherFactory(IExtensionMethodHandler extensionMethodHandler)
        {
            _extensionMethodHandler = extensionMethodHandler;
        }

        public IEnumerable<IMemberMatcher> GetMatchers(Type type)
        {
            foreach (var methodInfo in _extensionMethodHandler.GetExtensionMethodsFor(type))
            {
                DebugTrace.Trace(GetType().Name, "Added extension method matcher: " + methodInfo.Name);

                yield return new MethodNameMatcher(methodInfo);
            }
        }
    }
}