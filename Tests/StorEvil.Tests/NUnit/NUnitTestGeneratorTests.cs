using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;
using StorEvil.Nunit;
using StorEvil.Utility;

namespace StorEvil.NUnit
{
    [TestFixture]
    public class NUnitTestGeneratorTests : TestBase
    {
        protected static Scenario BuildScenario(string name, params string[] lines)
        {
            var lineNumber = 1;
            return new Scenario("test", lines.Select(line => new ScenarioLine { Text = line, LineNumber = lineNumber++}).ToArray());
        }

        [Test]
        public void Created_Test_Should_Call_When()
        {
            var s = BuildScenario("test", new[] { "When I do Something" });
            var story = new Story("test", "summary", new[] {s});
            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.When_I_Do_Something());
        }

        [Test]
        public void Created_Test_Should_Call_When_And_Then()
        {
            var s = BuildScenario("test", new[] { "When I do Something", "Then something should happen" });
            var story = new Story("test", "testing when and then both should be called", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.When_I_Do_Something());
            context.AssertWasCalled(x => x.Then_Something_Should_Happen());
        }

        [Test]
        public void Created_Test_Should_Inject_Unnamed_Parameter_For_Method()
        {
            var s = BuildScenario("test", new[] { "Given a user named Dave" });
            var story = new Story("test", "testing unnamed parameter injection", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.Given_A_User_Named("Dave"));
        }

        [Test]
        public void Created_Test_Should_Handle_And()
        {
            var s = BuildScenario("test", new[] { "Given a user named Dave", "and a dog named Fido" });
            var story = new Story("test", "testing junction of given clause using AND", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.Given_A_User_Named("Dave"));
            context.AssertWasCalled(x => x.Given_A_Dog_Named("Fido"));
        }

        [Test]
        public void Created_Test_Should_Inject_Parameters_By_Name()
        {
            // injecting parameters
            var s = BuildScenario("test", new[] { "A condition with 1 and test" });
            var story = new Story("test", "testing named parameter injection", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.A_Condition_With_intParam_and_stringParam(1, "test"));
        }

        [Test]
        public void Created_Test_Should_Parse_Currency()
        {
            // injecting parameters
            var s = BuildScenario("test", new[] { "A condition with $1 and test" });
            var story = new Story("test", "testing named parameter injection", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.A_Condition_With_intParam_and_stringParam(1, "test"));
        }

        [Test]
        public void Created_Test_Should_Dispose_Context()
        {
            // injecting parameters
            var s = BuildScenario("test", new[] { "foo" });
            var story = new Story("test", "foo", new[] {s});

            var context = new TestDisposableContext();

            CreateAndCallTestMethods<TestDisposableContext>(story, context);

            context.WasDisposed.ShouldEqual(true);
        }

        [Test]
        public void Created_Test_Should_Parse_DateTime()
        {
            // injecting parameters
            var s = BuildScenario("test", new[] { "A condition with 1/1/2009 param" });
            var story = new Story("test", "testing date time parsing", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.A_Condition_With_dateTime_param(new DateTime(2009, 1, 1)));
        }

        [Test]
        public void Created_Test_Should_Chain_Calls()
        {
            var s = BuildScenario("test", new[] { "Some sub context condition test" });

            var story = new Story("test", "testing chaining", new[] {s});

            var context = new TestContext {SubContext = new TestSubContext()};

            CreateAndCallTestMethods<TestContext>(story, context);
            context.SubContext.LastParameter.ShouldEqual("test");
        }

        [Test]
        public void Created_Test_Should_Support_Property_Invocation()
        {
            var s = BuildScenario("test", new[] { "sub context condition test" });

            var story = new Story("test", "testing property", new[] {s});

            var context = new TestContext {SubContext = new TestSubContext()};

            CreateAndCallTestMethods<TestContext>(story, context);
            context.SubContext.LastParameter.ShouldEqual("test");
        }

        [Test]
        public void Created_Test_Should_Support_Field_Invocation()
        {
            var s = BuildScenario("test", new[] { "sub context field condition test" });
            var story = new Story("test", "testing field invokation", new[] {s});

            var context = new TestContext {SubContext = new TestSubContext()};

            CreateAndCallTestMethods<TestContext>(story, context);

            context.SubContext.LastParameter.ShouldEqual("test");
        }

        [Test]
        public void Should_Throw_IgnoreException_When_Context_Does_Not_Implement_Methods()
        {
            var story = new Story("test", "testing assertion when no name match",
                                  new[] { BuildScenario("test", new[] { "subcontext bogus test" }) });

            var context = new TestContext();

            try
            {
                CreateAndCallTestMethods<TestContext>(story, context);
            }
            catch (Exception ex)
            {
                // we use reflection to invoke, hence inner exception
                if (ex.InnerException is IgnoreException)
                    return;

                throw;
            }

            Assert.Fail();
        }

        [Test]
        public void Should_Invoke_Extension_Method()
        {
            var s = BuildScenario("test", new[] { "sub context field name should equal Dave" });

            var story = new Story("test", "testing assertion when no name match", new[] {s});

            var context = new TestContext {SubContextField = new TestSubContext {Name = "Dave"}};

            CreateAndCallTestMethods<TestContext>(story, context);
        }

        [Test]
        public void Should_Handle_MultiLine_Param()
        {
            var s = BuildScenario("test", new[] { "foo\r\n|a|b|\r\n|c|d|" });

            var story = new Story("test", "testing assertion when no name match", new[] { s });

            var context = new MultiLineTestContext {};

            CreateAndCallTestMethods<MultiLineTestContext>(story, context);

            context.Bar[0][0].ShouldBe("a");
            context.Bar[0][1].ShouldBe("b");
            context.Bar[1][0].ShouldBe("c");
            context.Bar[1][1].ShouldBe("d");
        }

        [Test]
        public void Created_test_should_have_category_for_each_tag_on_scenario()
        {
            var scenario = BuildScenario("test", new[] {"When I Do Something"});
            scenario.Tags = new[] { "foo", "bar" };

            var s = new Story("test", "summary", new[] { scenario });
            Assembly a = CreateAssembly<object>(s);

            var testClass = a.GetTypes().First();

            var attributes = testClass
                .GetMethods().First()
                .GetCustomAttributes(typeof(CategoryAttribute), true)
                .Cast<CategoryAttribute>()
                .Select(x => x.Name);

            attributes.ElementsShouldEqual("foo", "bar");

        }

        private void CreateAndCallTestMethods<T>(Story story, T context)
        {
            Assembly a = CreateAssembly<T>(story);

            var fixtureType = a.GetTypes()[0];

            var fixture = Activator.CreateInstance(fixtureType);

            SetFixtureContext(fixture, context);

            foreach (var method in fixtureType.GetTestMethods())
                method.Invoke(fixture, new object[] {});
        }

        private Assembly CreateAssembly<T>(Story story)
        {
            var generator = GetNUnitGenerator();
            string code = "";
            IEnumerable<string> namespaces = new string[0];

            foreach (var sc in GetScenarios(story))
            {
                var nUnitTest = generator.GetTestFromScenario(sc, new StoryContext(typeof (T)));
                code += "        " + nUnitTest.Body + "\r\n";
                namespaces = namespaces.Union(nUnitTest.Namespaces).Distinct();
            }

            var format =
                @"
using System;
using NUnit.Framework;
using StorEvil;
{0}
namespace {1} {{   
    
    [TestFixture]
    public class {2} {{
        private StorEvil.Interpreter.ParameterConverters.ParameterConverter ParameterConverter = new StorEvil.Interpreter.ParameterConverters.ParameterConverter();
      
        public {3} _context;      
        {4}
    }}
}}";
            string formattedCode = string.Format(
                format,
                string.Join("\r\n", namespaces.Select(ns => "using " + ns + ";").ToArray()),
                GetType().Namespace, 
                "TestClass", 
                typeof (T).FullName,
                code.Replace("new " + typeof (T).FullName + "()", "_context"));

            return TestHelper.CreateAssembly(formattedCode);
        }

        private IEnumerable<Scenario> GetScenarios(Story story)
        {
            return story.Scenarios.OfType<Scenario>();
        }

        private NUnitTestMethodGenerator GetNUnitGenerator()
        {
            var ext = new ExtensionMethodHandler();

            return
                new NUnitTestMethodGenerator(
                    new CSharpMethodInvocationGenerator(new ScenarioInterpreter(new InterpreterForTypeFactory(ext))));
        }

        private static void SetFixtureContext<T>(object fixture, T context)
        {
            var fields = fixture.GetType().GetFields();
            var field = fields.First(f => f.FieldType == typeof (T));

            field.SetValue(fixture, context);
        }
    }

    public class MultiLineTestContext
    {
        public void Foo(string[][] bar)
        {
            Bar = bar;
        }

        public string[][] Bar { get; set; }
    }

    public class TestDisposableContext : IDisposable
    {
        public void Dispose()
        {
            WasDisposed = true;
        }

        public void Foo()
        {
        }

        public bool WasDisposed { get; set; }
    }
}