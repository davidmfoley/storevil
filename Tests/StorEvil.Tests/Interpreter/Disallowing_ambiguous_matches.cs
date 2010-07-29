using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StorEvil.InPlace;
using StorEvil.Interpreter.ParameterConverter_Specs;
using StorEvil.Utility;

namespace StorEvil.Interpreter.Ambiguous_Match_Resolution
{

    [TestFixture]
    public class Most_recently_used_ambiguous_match_resolution
    {
        [SetUp]
        public void SetUpContext()
        {
            MostRecentlyUsedContext.Reset();
        }
        private MostRecentlyUsedContext Resolver = new MostRecentlyUsedContext();
      
        [Test]
        public void when_neither_candidate_context_has_been_used_throws_an_exception()
        {
            var invocationChains = GetTestInvocationChains();
            
            Expect.ThisToThrow<MatchResolutionException>(() => Resolver.ResolveMatch("Ambiguous", invocationChains));
        }

        
        [Test]
        public void when_one_candidate_context_has_been_used_it_is_chosen()
        {
            var invocationChains = GetTestInvocationChains();

            SimulateInvocation(new FooContext());

            var chosenChain = Resolver.ResolveMatch("Ambiguous", invocationChains);

            chosenChain.Invocations.First().MemberInfo.DeclaringType.ShouldEqual(typeof(FooContext));

        }

      
        [Test]
        public void when_both_candidate_contexts_have_been_used_the_most_recently_used_is_chosen()
        {
            var invocationChains = GetTestInvocationChains();

            SimulateInvocation(new FooContext());
            SimulateInvocation(new BarContext());
            SimulateInvocation(new FooContext());

            var chosenChain = Resolver.ResolveMatch("Ambiguous", invocationChains);
            chosenChain.Invocations.First().MemberInfo.DeclaringType.ShouldEqual(typeof(FooContext));

        }

        private void SimulateInvocation(object context)
        {
            MostRecentlyUsedContext.MemberInvoker_OnMemberInvoked(this, new MemberInvokedHandlerArgs { Context = context });
        }

        private IEnumerable<InvocationChain> GetTestInvocationChains()
        {
            return new[]
                       {
                           GetTestInvocationChain<FooContext>("Ambiguous"), 
                           GetTestInvocationChain<BarContext>("Ambiguous") 
                       };
        }


        private InvocationChain GetTestInvocationChain<T>(string name)
        {
            return new InvocationChain(GetTestInvocation<T>(name));
        }

        private Invocation GetTestInvocation<T>(string name)
        {
            return new Invocation(typeof(T).GetMethod(name), new object[0], new string[0], name);
        }

        public class FooContext
        {
            public void Ambiguous() { }
        }

        public class BarContext
        {
            public void Ambiguous() { }
        }

    }

    internal class MostRecentlyUsedContext : IAmbiguousMatchResolver
    {
        private static List<Type> _mruTypes = new List<Type>();

        static MostRecentlyUsedContext()
        {
            MemberInvoker.OnMemberInvoked += MemberInvoker_OnMemberInvoked;
        }

        public static void Reset()
        {
            _mruTypes = new List<Type>();
        }
        public static void MemberInvoker_OnMemberInvoked(object sender, MemberInvokedHandlerArgs args)
        {
            var type = args.Context.GetType();
            
            _mruTypes = new[] {type}.Union(_mruTypes.Where(x => x != type)).ToList();
        }

        public InvocationChain ResolveMatch(string line, IEnumerable<InvocationChain> invocationChains)
        {
            var positions = invocationChains.Select(x => new {Position = GetPosition(x), Value = x});

            var pair = positions.Where(x => x.Position >= 0).OrderBy(x => x.Position).FirstOrDefault();

            if (pair == null)
                throw new MatchResolutionException();

            return pair.Value;


        }

        private int GetPosition(InvocationChain invocationChain)
        {
            var declaringType = invocationChain.Invocations.First().MemberInfo.DeclaringType;

            return _mruTypes.IndexOf(declaringType);
        }
    }

    [TestFixture]
    public class Disallowing_ambiguous_matches
    {
        private DisallowAmbiguousMatches Resolver = new DisallowAmbiguousMatches();
        private MatchResolutionException CaughtException;

        [SetUp]
        public void SetUpContext()
        {
            var chains = new[]{
                                  new InvocationChain(new Invocation(typeof(AmbiguousTestClass).GetMethod("Foo"), new object[0], new string[0], "foo bar baz")),
                                  new InvocationChain()
                              };

            CaughtException = Expect.ThisToThrow<MatchResolutionException>(() => Resolver.ResolveMatch("foo bar baz", chains));

        }

        [Test]
        public void throws_exception_with_invocation_info()
        {
            CaughtException.Message.ShouldContain("AmbiguousTestClass.Foo");
        }
    }
}