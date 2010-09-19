using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Context.Matchers;
using StorEvil.Core;
using StorEvil.Interpreter;

namespace StorEvil.Glossary
{
   
    public class Step_describer_spec
    {       
        protected StepDescription GetPropertyMatcherResult(string propertyName)
        {
            var matcher = new PropertyOrFieldNameMatcher(typeof(ExampleContext).GetProperty(propertyName));
            return Describe(matcher);
        }

        protected StepDescription GetRegexMatcherResult(string methodName)
        {
            var matcher =
                new RegExMatcherFactory()
                    .GetMatchers(typeof(ExampleContext))
                    .First(x => x.MemberInfo.Name == methodName);

            return Describe(matcher);
        }

        protected StepDescription GetMethodNameMatcherResult(string methodName)
        {
            var matcher = new MethodNameMatcher(typeof(ExampleContext).GetMethod(methodName));
            return Describe(matcher);
        }

        private StepDescription Describe(IMemberMatcher matcher)
        {
            var def = new StepDefinition { DeclaringType = typeof(ExampleContext), Matcher = matcher };
            return new StepDescriber().Describe(def);
        }

        class ExampleContext
        {
            public string Example_property
            {
                get;
                set;
            }

            public void This_is_an_example() { }

            public void This_is_an_example_with_a_parameter(int parameter)
            {
            }

            [ContextRegex("This is an example of a regex")]
            public void RegExExample() { }

            [ContextRegex("This is an example of a regex with a (.+)")]
            public void RegExExampleWithParameter(string parameter) { }

            [ContextRegex("This is an example of a regex with a (.+) embedded")]
            public void RegExExampleWithEmbeddedParameter(int parameter) { }
        }
    }

    [TestFixture]
    public class Describing_reflection_step : Step_describer_spec
    {
        [Test]
        public void can_describe_a_reflection_step()
        {
            var result = GetMethodNameMatcherResult("This_is_an_example");

            result.Description.ShouldEqual("This is an example");
        }

        [Test]
        public void can_describe_a_reflection_step_with_a_parameter()
        {
            var result = GetMethodNameMatcherResult("This_is_an_example_with_a_parameter");

            result.Description.ShouldEqual("This is an example with a <int parameter>");

        }
    }

    [TestFixture]
    public class Describing_regex_step : Step_describer_spec
    {
        private StepDescription Result;

        [SetUp]
        public void SetUpContext()
        {
            Result = GetRegexMatcherResult("RegExExample");
        }
        [Test]
        public void can_describe_a_regex_step()
        {          
            Result.Description.ShouldEqual("This is an example of a regex");
        }

        [Test]
        public void can_get_spans_for_a_regex_step()
        {           
            Result.Spans.Count().ShouldBe(1);
            Result.Spans.First().ShouldBeOfType<TextSpan>();
            Result.Spans.First().Text.ShouldBe("This is an example of a regex");
        }
    }


    [TestFixture]
    public class Describing_regex_step_with_parameter : Step_describer_spec
    {
        private StepDescription Result;

        [SetUp]
        public void SetUpContext()
        {
            Result = GetRegexMatcherResult("RegExExampleWithParameter");
        }

        [Test]
        public void can_describe_a_regex_step_with_parameter()
        { 
            Result.Description.ShouldEqual("This is an example of a regex with a <string parameter>");
        }

        [Test]
        public void Should_have_two_spans()
        {
            Result.Spans.Count().ShouldBe(2);
        }
    }


    [TestFixture]
    public class Describing_regex_step_with_embedded_parameter : Step_describer_spec
    {
        private StepDescription Result;

        [SetUp]
        public void SetUpContext()
        {
            Result = GetRegexMatcherResult("RegExExampleWithEmbeddedParameter");
        }

        [Test]
        public void can_describe_a_regex_step_with_embedded_parameter()
        {
            Result.Description.ShouldEqual("This is an example of a regex with a <int parameter> embedded");           
        }

        [Test]
        public void has_three_spans()
        {
            Result.Spans.Count().ShouldBe(3);   
        }

        [Test]
        public void first_span_is_text()
        {
            var firstSpan = Result.Spans.First() as TextSpan;
            firstSpan.Text.ShouldBe("This is an example of a regex with a ");
        }

        [Test]
        public void second_span_is_parameter()
        {
            var secondSpan = Result.Spans.ElementAt(1) as ParameterSpan;
            secondSpan.Name.ShouldBe("parameter");
            secondSpan.ParameterType.ShouldBe(typeof(int));

        }

        [Test]
        public void third_span_is_text()
        {
            var thirdSpan = Result.Spans.ElementAt(2) as TextSpan;
            thirdSpan.Text.ShouldBe(" embedded");
        }

    }

    [TestFixture]
    public class Describing_property_step : Step_describer_spec
    {
        [Test]
        public void can_describe_a_property_step()
        {
            var result = GetPropertyMatcherResult("Example_property");

            result.Description.ShouldEqual("Example property");
        }
    }

    [TestFixture]
    public class Describing_steps_with_children : Step_describer_spec
    {
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

            var result = new StepDescriber().Describe(step);
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

            var result = new StepDescriber().Describe(step);
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

    }
}