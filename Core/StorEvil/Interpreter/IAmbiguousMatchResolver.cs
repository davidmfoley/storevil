using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using StorEvil.Events;
using StorEvil.InPlace;

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
            throw  MatchResolutionException.Build(invocationChains);
        }

       
    }

    public class MostRecentlyUsedContext : IAmbiguousMatchResolver, IEventHandler<MatchFoundEvent>, IEventHandler<ScenarioStartingEvent>
    {
        private static List<Type> _mruTypes = new List<Type>();

        public MostRecentlyUsedContext()
        {
            StorEvilEvents.Bus.Register(this);
        }

        public static void Reset()
        {
            _mruTypes = new List<Type>();
        }
     
        public InvocationChain ResolveMatch(string line, IEnumerable<InvocationChain> invocationChains)
        {
            var positions = invocationChains.Select(x => new { Position = GetPosition(x), Value = x });

            var pair = positions.Where(x => x.Position >= 0).OrderBy(x => x.Position).FirstOrDefault();

            if (pair == null)
                throw MatchResolutionException.Build(invocationChains);

            return pair.Value;
        }

        private int GetPosition(InvocationChain invocationChain)
        {
            var declaringType = invocationChain.Invocations.First().MemberInfo.DeclaringType;

            return _mruTypes.IndexOf(declaringType);
        }

        public void Handle(MatchFoundEvent eventToHandle)
        {
            var type = eventToHandle.Member.DeclaringType;

            _mruTypes = new[] { type }.Union(_mruTypes.Where(x => x != type)).ToList();
        }

        public void Handle(ScenarioStartingEvent eventToHandle)
        {
            _mruTypes = new List<Type>();
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

        public static Exception Build(IEnumerable<InvocationChain> invocationChains)
        {
            return new MatchResolutionException(GetMessage(invocationChains));
        }

        private static string GetMessage(IEnumerable<InvocationChain> invocationChains)
        {
            var names = invocationChains.Select(GetInvocationDescription);
            return "Could not select between\r\n -" + string.Join("\r\n -", names.ToArray());
        }

        private static string GetInvocationDescription(InvocationChain chain)
        {
            var members = chain.Invocations.Select(x => x.MemberInfo.DeclaringType.Name + "." + x.MemberInfo.Name);
            return string.Join(", ", members.ToArray());
        }
    }
}