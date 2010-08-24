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
    public class Context_Type_Factory
    {
        [Test]
        public void Should_filter_out_get_property_methods()
        {
            var factory = new ContextTypeFactory(new ExtensionMethodHandler(new AssemblyRegistry()));
            var contextType = factory.GetWrapper(typeof(TypeWithProperty));
            contextType.MemberMatchers.Any(x=>x.MemberInfo.Name=="get_Foo").ShouldEqual(false);
        }
    }

    public class TypeWithProperty
    {
        public string Foo { get; set; }
    }

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
          
            Provider = new StepProvider(FakeAssemblyRegistry, new ContextTypeFactory(FakeExtensionMethodHandler));
        }

        [Test]
        public void returns_one_step_for_each_member()
        {
            GivenContextTypes(typeof (TestContext));
            GivenExtensionMethods();
            Provider.GetSteps().Count().ShouldBe(1);
        }

        [Test]
        public void step_has_type_set()
        {
            GivenContextTypes(typeof(TestContext));
            GivenExtensionMethods();
            var step = Provider.GetSteps().First();
            step.DeclaringType.ShouldEqual(typeof (TestContext));
        }

        [Test]
        public void step_has_matcher()
        {
            GivenContextTypes(typeof(TestContext));
            GivenExtensionMethods();
            var step = Provider.GetSteps().First();
            step.Matcher.ShouldBeOfType<MethodNameMatcher>();
            step.Matcher.MemberInfo.Name.ShouldBe("Context_method_example");
        }

        [Test]
        public void Excludes_extension_methods()
        {
            GivenContextTypes(typeof(TestContext));
            GivenExtensionMethods(typeof(ExampleExtension).GetMethod("Extension"));

            Provider = new StepProvider(FakeAssemblyRegistry, new ContextTypeFactory(FakeExtensionMethodHandler));

            Provider.GetSteps().Count().ShouldEqual(1);
        }

        private void GivenExtensionMethods(params MethodInfo[] methods)
        {
            FakeExtensionMethodHandler.Stub(x => x.GetExtensionMethodsFor(Arg<Type>.Is.Anything)).Return(methods);
        }

        private void GivenContextTypes(params Type[] contextTypes)
        {
            FakeAssemblyRegistry.Stub(x => x.GetTypesWithCustomAttribute<ContextAttribute>())
                .Return(contextTypes);
        }

     

        class TestContext
        {
            public object Context_method_example()
            {
                return null;
            }
        }

 
    }

    public static class ExampleExtension
    {
        public static void Extension(this object foo)
        {
        }
    }
}