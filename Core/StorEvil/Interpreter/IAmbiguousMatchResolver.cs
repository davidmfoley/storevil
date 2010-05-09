using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace StorEvil.Interpreter
{
    public interface IAmbiguousMatchResolver
    {
        InvocationChain ResolveMatch(string line, IEnumerable<InvocationChain> invocationChains);
    }

    public class DisallowAmbiguousMatches : IAmbiguousMatchResolver
    {
        public InvocationChain ResolveMatch(string line, IEnumerable<InvocationChain> invocationChains)
        {

            throw new MatchResolutionException(GetMessage(invocationChains));
        }

        private string GetMessage(IEnumerable<InvocationChain> invocationChains)
        {
            var names = invocationChains.Select(GetInvocationDescription);
            return "Could not select between\r\n -" + string.Join("\r\n -", names.ToArray());
        }

        private string GetInvocationDescription(InvocationChain chain)
        {
            var members = chain.Invocations.Select(x => x.MemberInfo.DeclaringType.Name + "." + x.MemberInfo.Name);
            return string.Join(", ", members.ToArray());
        }
    }

    [Serializable]
    public class MatchResolutionException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public MatchResolutionException()
        {
        }

        public MatchResolutionException(string message) : base(message)
        {
        }

        public MatchResolutionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MatchResolutionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}