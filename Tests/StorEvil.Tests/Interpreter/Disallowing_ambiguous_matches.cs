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
            var method = context.GetType().GetMethod("Ambiguous");
            StorEvilEvents.Bus.Raise(new MatchFoundEvent {Member = method});
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