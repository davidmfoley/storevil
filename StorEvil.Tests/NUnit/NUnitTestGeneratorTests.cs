using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Nunit;

namespace StorEvil.NUnit
{
    
    [TestFixture]
    public class NUnitTestGeneratorTests : TestBase
    {
        [Test]
        public void Created_Test_Should_Call_When()
        {
            var s = new Scenario("test", new[] {"When I do Something"});
            var story = new Story("test", "summary", new[] {s});
            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.When_I_Do_Something());
        }

        [Test]
        public void Created_Test_Should_Call_When_And_Then()
        {
            var s = new Scenario("test", new[] {"When I do Something", "Then something should happen"});
            var story = new Story("test", "testing when and then both should be called", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.When_I_Do_Something());
            context.AssertWasCalled(x => x.Then_Something_Should_Happen());
        }

        [Test]
        public void Created_Test_Should_Inject_Unnamed_Parameter_For_Method()
        {
            var s = new Scenario("test", new[] {"Given a user named Dave"});
            var story = new Story("test", "testing unnamed parameter injection", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.Given_A_User_Named("Dave"));
        }

        [Test]
        public void Created_Test_Should_Handle_And()
        {
            var s = new Scenario("test", new[] {"Given a user named Dave", "and a dog named Fido"});
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
            var s = new Scenario("test", new[] {"A condition with 1 and test"});
            var story = new Story("test", "testing named parameter injection", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.A_Condition_With_intParam_and_stringParam(1, "test"));
        }

        [Test]
        public void Created_Test_Should_Parse_Currency()
        {
            // injecting parameters
            var s = new Scenario("test", new[] {"A condition with $1 and test"});
            var story = new Story("test", "testing named parameter injection", new[] {s});

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.A_Condition_With_intParam_and_stringParam(1, "test"));
        }

        [Test]
        public void Created_Test_Should_Parse_DateTime()
        {
            // injecting parameters
            var s = new Scenario("test", new[] { "A condition with 1/1/2009 param" });
            var story = new Story("test", "testing date time parsing", new[] { s });

            var context = Fake<TestContext>();

            CreateAndCallTestMethods<TestContext>(story, context);

            context.AssertWasCalled(x => x.A_Condition_With_dateTime_param(new DateTime(2009,1,1)));
        }

        [Test]
        public void Created_Test_Should_Chain_Calls()
        {
            var s = new Scenario("test", new[] {"Some sub context condition test"});

            var story = new Story("test", "testing chaining", new[] {s});

            var context = new TestContext {SubContext = new TestSubContext()};

            CreateAndCallTestMethods<TestContext>(story, context);
            context.SubContext.LastParameter.ShouldEqual("test");
        }

        [Test]
        public void Created_Test_Should_Support_Property_Invocation()
        {
            var s = new Scenario("test", new[] {"sub context condition test"});

            var story = new Story("test", "testing property", new[] {s});

            var context = new TestContext {SubContext = new TestSubContext()};

            CreateAndCallTestMethods<TestContext>(story, context);
            context.SubContext.LastParameter.ShouldEqual("test");
        }

        [Test]
        public void Created_Test_Should_Support_Field_Invocation()
        {
            var s = new Scenario("test", new[] {"sub context field condition test"});
            var story = new Story("test", "testing field invokation", new[] {s});

            var context = new TestContext {SubContext = new TestSubContext()};

            CreateAndCallTestMethods<TestContext>(story, context);

            context.SubContext.LastParameter.ShouldEqual("test");
        }

        [Test]
        public void Should_Throw_Exception_When_Context_Does_Not_Implement_Methods()
        {
            var story = new Story("test", "testing assertion when no name match",
                                  new[] {new Scenario("test", new[] {"subcontext bogus test"})});

            var context = new TestContext();

            try
            {
                CreateAndCallTestMethods<TestContext>(story, context);
            }
            catch (Exception ex)
            {
                // we use reflection to invoke, hence inner exception
                if (ex.InnerException is AssertionException)
                    return;

                throw;
            }

            Assert.Fail();
        }

        [Test]
        public void Should_Invoke_Extension_Method()
        {
            var s = new Scenario("test", new[] {"sub context field name should equal Dave"});

            var story = new Story("test", "testing assertion when no name match", new[] {s});

            var context = new TestContext {SubContextField = new TestSubContext {Name = "Dave"}};

            CreateAndCallTestMethods<TestContext>(story, context);
        }

        private void CreateAndCallTestMethods<T>(Story story, object context)
        {
            var generator = GetNUnitGenerator();
            string code = "";
            foreach (var sc in GetScenarios(story))
                code += "        " + generator.GetTestFromScenario(sc, new StoryContext(typeof (T))).Body + "\r\n";

            var format =
                @"
using System;
using NUnit.Framework;
using StorEvil;
namespace {0} {{   
    
    [TestFixture]
    public class {1} {{
        public TestContext _context;      
        {3}
    }}
}}";
            string formattedCode = string.Format(
                format, GetType().Namespace, "TestClass", "TestContext",
                code.Replace("new StorEvil.TestContext()", "_context"));

            Assembly a = TestHelper.CreateAssembly(formattedCode);

            var fixtureType = a.GetTypes()[0];

            var fixture = Activator.CreateInstance(fixtureType);

            SetFixtureContext(fixture, context);

            foreach (var method in fixtureType.GetTestMethods())
                method.Invoke(fixture, new object[] {});
        }

        private IEnumerable<Scenario> GetScenarios(Story story)
        {
            return story.Scenarios.OfType<Scenario>();
        }

        private NUnitTestMethodGenerator GetNUnitGenerator()
        {
            var ext = new ExtensionMethodHandler();
           
            return new NUnitTestMethodGenerator(new CSharpMethodInvocationGenerator(new ScenarioInterpreter(new InterpreterForTypeFactory(ext))));
        }

        private static void SetFixtureContext(object fixture, object context)
        {
            var fieldInfo = fixture.GetType().GetField("_context");
            fieldInfo.SetValue(fixture, context);
        }
    }
}