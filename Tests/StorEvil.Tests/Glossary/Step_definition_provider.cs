using System;
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
    public class Step_definition_provider
    {
        private AssemblyRegistry FakeAssemblyRegistry;
        private StepProvider Provider;
        private IExtensionMethodHandler FakeExtensionMethodHandler;

        [SetUp]
        public void SetUpContext()
        {
            FakeAssemblyRegistry = MockRepository.GenerateStub<AssemblyRegistry>();
            FakeExtensionMethodHandler = MockRepository.GenerateStub<IExtensionMethodHandler>();
            FakeExtensionMethodHandler.Stub(x => x.GetExtensionMethodsFor(Arg<Type>.Is.Anything)).Return(new MethodInfo[0]);

            Provider = new StepProvider(FakeAssemblyRegistry, new ContextTypeFactory(FakeExtensionMethodHandler));
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