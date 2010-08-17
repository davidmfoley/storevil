using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Context;
using StorEvil.NUnit;
using StorEvil.Utility;

namespace StorEvil.Interpreter.Ambiguous_Match_Resolution
{
    [TestFixture]
    public class Resolving_ambiguous_matches
    {
        private IAmbiguousMatchResolver FakeResolver;
        private InvocationChain ChainReturnedFromResolver;
        private InvocationChain Result;

        [TestFixtureSetUp]
        public void SetupContext()
        {
            FakeResolver = MockRepository.GenerateMock<IAmbiguousMatchResolver>();

            var factory = new InterpreterForTypeFactory(new ExtensionMethodHandler(new AssemblyRegistry()));
      
            ChainReturnedFromResolver = new InvocationChain();
            FakeResolver.Stub(x => x.ResolveMatch("", null)).IgnoreArguments().Return(ChainReturnedFromResolver);

            ScenarioInterpreter interpreter = new ScenarioInterpreter(factory, FakeResolver);

            Result = interpreter.GetChain(
                new ScenarioContext(new StoryContext(new FakeSessionContext()), new Type[] {typeof (AmbiguousTestClass)},
                                    new Dictionary<Type, object>()),
                "Foo bar baz");
        }

        [Test]
        public void Interpreter_uses_the_resolver()
        {
            FakeResolver.AssertWasCalled(x=>x.ResolveMatch(Arg<string>.Is.Anything, Arg<IEnumerable<InvocationChain>>.Matches(c => c.Count() == 2)));
        }

        [Test]
        public void returns_the_resolved_chain()
        {
            Result.ShouldBe(ChainReturnedFromResolver);
        }
    }

    [TestFixture]
    public class When_an_ambiguous_match_is_possible
    {
        private IEnumerable<InvocationChain> Chains;

        [TestFixtureSetUp]
        public void SetupContext()
        {
            var interpreterForTypeFactory = new InterpreterForTypeFactory(new ExtensionMethodHandler(new AssemblyRegistry()));
            var interpreter = new ScenarioInterpreterForType(typeof(AmbiguousTestClass), new MethodInfo[] {},interpreterForTypeFactory);

           Chains = interpreter.GetChains("Foo bar baz");
        }

        [Test]
        public void scenario_interpreter_should_return_both_matches()
        {
           Chains.Count().ShouldEqual(2);
        }
    }

    public class AmbiguousTestClass
    {
        public AmbiguousTestSubClass Foo()
        {
            return new AmbiguousTestSubClass();
        }

        public void Foo_Bar_Baz() {}
    }

    public class AmbiguousTestSubClass
    {
        public void Bar_Baz() {}
    }

}