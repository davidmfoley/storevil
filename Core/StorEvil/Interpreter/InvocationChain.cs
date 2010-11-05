using System.Collections.Generic;

namespace StorEvil.Interpreter
{
    public class InvocationChain
    {
        public InvocationChain(params Invocation[] invocations)
        {
            Invocations = invocations;
        }

        public IEnumerable<Invocation> Invocations { get; set; }
    }
}