using System;
using System.Linq;

namespace StorEvil.Interpreter
{
    public class ContextTypeFactory
    {
        private IMatcherFactory[] _matcherFactories;
        private readonly IExtensionMethodHandler _extensionMethodHandler;

        public ContextTypeFactory(IExtensionMethodHandler extensionMethodHandler)
        {
            _extensionMethodHandler = extensionMethodHandler;
            _matcherFactories = new IMatcherFactory[] { new RegExMatcherFactory(), new ReflectionMatcherFactory(), new ExtensionMatcherFactory(_extensionMethodHandler) };
        }

        public ContextType GetWrapper(Type type)
        {
            var matchers = _matcherFactories.SelectMany(x => x.GetMatchers(type));

            return new ContextType(type, matchers);
        }

     
    }
}