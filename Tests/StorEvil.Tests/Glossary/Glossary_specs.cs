using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Context;
using StorEvil.Context.Matchers;
using StorEvil.Core;
using StorEvil.Interpreter;

namespace StorEvil.Glossary
{
    [TestFixture]
    public class glossary_job
    {
        private StorEvilGlossaryJob Job;
        private IStepProvider _fakeStepProvider;
        private IStepDescriber _fakeStepDescriber;

        [SetUp]
        public void SetUpContext()
        {
            _fakeStepProvider = MockRepository.GenerateStub<IStepProvider>();
            _fakeStepDescriber = MockRepository.GenerateStub<IStepDescriber>();
            Job = new StorEvilGlossaryJob(_fakeStepProvider, _fakeStepDescriber);
        }

        [Test]
        public void returns_0()
        {
            StepProviderReturns();
            Job.Run().ShouldEqual(0);
        }

        [Test]
        public void sends_step_defs_to_formatter()
        {
            var definition1 = new StepDefinition();
            var definition2 = new StepDefinition();

            StepProviderReturns(definition1, definition2);

            Job.Run();

            _fakeStepDescriber.AssertWasCalled(x => x.Describe(definition1));
            _fakeStepDescriber.AssertWasCalled(x => x.Describe(definition2));
        }

        private void StepProviderReturns(params StepDefinition[] definitions)
        {
            _fakeStepProvider
                .Stub(x => x.GetSteps())
                .Return(definitions);
        }
    }

    [TestFixture]
    public class Step_definition_provider
    {
        private AssemblyRegistry FakeAssemblyRegistry;
        private StepProvider Provider;

        [SetUp]
        public void SetUpContext()
        {
            FakeAssemblyRegistry = MockRepository.GenerateStub<AssemblyRegistry>();
            Provider = new StepProvider(FakeAssemblyRegistry);
        }

        [Test]
        public void returns_one_step_for_each_member()
        {
            GivenContextTypes(typeof (TestContext));

            Provider.GetSteps().Count().ShouldBe(1);
        }

        [Test]
        public void step_has_type_set()
        {
            GivenContextTypes(typeof(TestContext));

            var step = Provider.GetSteps().First();
            step.DeclaringType.ShouldEqual(typeof (TestContext));
        }

        [Test]
        public void step_has_matcher()
        {
            GivenContextTypes(typeof(TestContext));

            var step = Provider.GetSteps().First();
            step.Matcher.ShouldBeOfType<MethodNameMatcher>();
            step.Matcher.MemberInfo.Name.ShouldBe("Context_method_example");
        }

        private void GivenContextTypes(params Type[] contextTypes)
        {
            FakeAssemblyRegistry.Stub(x => x.GetTypesWithCustomAttribute<ContextAttribute>())
                .Return(contextTypes);
        }

        class TestContext
        {
            public void Context_method_example()
            {
            }
        }
    }

    public class StepProvider : IStepProvider
    {
        private readonly AssemblyRegistry _assemblyRegistry;

        public StepProvider(AssemblyRegistry assemblyRegistry)
        {
            _assemblyRegistry = assemblyRegistry;
        }

        public IEnumerable<StepDefinition> GetSteps()
        {
            foreach (var type in _assemblyRegistry.GetTypesWithCustomAttribute<ContextAttribute>())
            {
                var wrapper = new ContextTypeWrapper(type, new MethodInfo[0]);
                foreach (var memberMatcher in wrapper.MemberMatchers)
                {
                    yield return BuildStepDefinition(type, memberMatcher);
                }
            }
        }

        private StepDefinition BuildStepDefinition(Type declaringType, IMemberMatcher memberMatcher)
        {
           return new StepDefinition {DeclaringType  = declaringType, Matcher = memberMatcher};
        }
    }
}