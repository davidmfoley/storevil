using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Context;
using StorEvil.Context.Matchers;
using StorEvil.Core;

namespace StorEvil.Glossary
{
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
}