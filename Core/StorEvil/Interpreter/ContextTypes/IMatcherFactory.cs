using System;
using System.Collections.Generic;
using StorEvil.Context.Matchers;

namespace StorEvil.Interpreter
{
    public interface IMatcherFactory
    {
        IEnumerable<IMemberMatcher> GetMatchers(Type type);
    }
}