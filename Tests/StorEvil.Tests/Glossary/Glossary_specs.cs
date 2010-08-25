using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Context.Matchers;
using StorEvil.Context.WordFilters;
using StorEvil.Core;
using StorEvil.Events;
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
            Job = new StorEvilGlossaryJob(_fakeStepProvider, _fakeStepDescriber, new EventBus(), new NoOpGlossaryFormatter());
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
            _fakeStepDescriber.Stub(x => x.Describe(Arg<StepDefinition>.Is.Anything)).Return(new StepDescription());
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
    public class Step_describer
    {
        private StepDescriber Describer;

        [SetUp]
        public void SetUpContext()
        {
            Describer = new StepDescriber();
        }

        [Test]
        public void can_describe_a_reflection_step()
        {
            string result = GetMethodNameMatcherResult("This_is_an_example");

            result.ShouldEqual("This is an example");
        }

        [Test]
        public void can_describe_a_reflection_step_with_a_parameter()
        {
            string result = GetMethodNameMatcherResult("This_is_an_example_with_a_parameter");

            result.ShouldEqual("This is an example with a <int parameter>");
        }

        [Test]
        public void can_describe_a_regex_step()
        {
            string result = GetRegexMatcherResult("RegExExample");

            result.ShouldEqual("This is an example of a regex");
        }

        [Test]
        public void can_describe_a_regex_step_with_parameter()
        {
            string result = GetRegexMatcherResult("RegExExampleWithParameter");

            result.ShouldEqual("This is an example of a regex with a <string parameter>");
        }

        [Test]
        public void can_describe_a_regex_step_with_embedded_parameter()
        {
            string result = GetRegexMatcherResult("RegExExampleWithEmbeddedParameter");

            result.ShouldEqual("This is an example of a regex with a <int parameter> embedded");
        }

        [Test]
        public void can_describe_a_property_step()
        {
            string result = GetPropertyMatcherResult("Example_property");

            result.ShouldEqual("Example property");
        }

        [Test, Ignore("rethinking this")]
        public void can_describe_a_step_with_a_child()
        {
            var childStep = new StepDefinition
                                {
                                    DeclaringType = typeof (ExampleChildContext),
                                    Matcher =  new MethodNameMatcher(typeof (ExampleChildContext).GetMethod("Bar"))
                                };

            var step = new StepDefinition
                           {
                               Children = new[] {childStep},
                               DeclaringType = typeof(ExampleParentContext),
                               Matcher = new MethodNameMatcher(typeof (ExampleParentContext).GetMethod("Foo"))
                           };

            var result = Describer.Describe(step);
            result.Description.ShouldEqual("Foo");
            result.ChildDescription.ShouldEqual("    Bar");
        }

        [Test, Ignore("rethinking this")]
        public void can_describe_a_step_with_multiple_levels_of_children()
        {
            var grandchildStep = new StepDefinition
            {
                DeclaringType = typeof(ExampleGrandChildContext),
                Matcher = new MethodNameMatcher(typeof(ExampleGrandChildContext).GetMethod("Baz"))
            };

            var childStep = new StepDefinition
            {
                Children = new[] { grandchildStep },
                DeclaringType = typeof(ExampleChildContext),
                Matcher = new MethodNameMatcher(typeof(ExampleChildContext).GetMethod("Bar"))
            };

            var step = new StepDefinition
            {
                Children = new[] { childStep },
                DeclaringType = typeof(ExampleParentContext),
                Matcher = new MethodNameMatcher(typeof(ExampleParentContext).GetMethod("Foo"))
            };

            var result = Describer.Describe(step);
            result.Description.ShouldEqual("Foo");
            result.ChildDescription.ShouldEqual("    Bar\r\n        Baz");
        }

        public class ExampleParentContext
        {
            public ExampleChildContext Foo()
            {
                return null;
            }
        }

        public class ExampleChildContext
        {
            public ExampleGrandChildContext Bar()
            {
                return null;
            }
        }


        public class ExampleGrandChildContext
        {
            public void Baz()
            {
            }
        }

        private string GetPropertyMatcherResult(string propertyName)
        {
            var matcher = new PropertyOrFieldNameMatcher(typeof (ExampleContext).GetProperty(propertyName));
            return Describe(matcher);
        }

        private string GetRegexMatcherResult(string methodName)
        {
            var matcher =
                new RegExMatcherFactory()
                    .GetMatchers(typeof (ExampleContext))
                    .First(x => x.MemberInfo.Name == methodName);
           
            return Describe(matcher);
        }

        private string GetMethodNameMatcherResult(string methodName)
        {
            var matcher = new MethodNameMatcher(typeof (ExampleContext).GetMethod(methodName));
            return Describe(matcher);
        }

        private string Describe(IMemberMatcher matcher)
        {
            var def = new StepDefinition {DeclaringType = typeof (ExampleContext), Matcher = matcher};
            return Describer.Describe(def).Description;
        }

        class ExampleContext
        {
            public string Example_property
            {
                get;
                set;
            }

            public void This_is_an_example() {}

            public void This_is_an_example_with_a_parameter(int parameter)
            {
            }

            [ContextRegex("This is an example of a regex")]
            public void RegExExample() {}

            [ContextRegex("This is an example of a regex with a (.+)")]
            public void RegExExampleWithParameter(string parameter) { }

            [ContextRegex("This is an example of a regex with a (.+) embedded")]
            public void RegExExampleWithEmbeddedParameter(int parameter) { }
        }
    }

}