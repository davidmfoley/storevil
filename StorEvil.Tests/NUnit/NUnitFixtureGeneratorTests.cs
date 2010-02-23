using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.Nunit;

namespace StorEvil.NUnit
{
    [TestFixture]
    public class NUnitFixtureGeneratorTests : TestBase
    {
       
        [Test]
        public void Should_Create_One_Test_For_A_Single_Scenario()
        {
            var s = new Scenario("test", new[] { "When I Do Something" });
            var story = new Story("test", "summary", new Scenario[] { s });

            Assembly a = BuildTestAssembly(story);
            var fixture = a.GetTypes()[0];
            fixture.GetTestMethods().Count().ShouldEqual(1);
        }

        [Test]
        public void Should_Compile_When_No_Scenarios()
        {
            var s = new Story("test", "summary", new Scenario[] { });
            Assembly a = BuildTestAssembly(s);
        }


        [Test]
        public void Should_Contain_A_Single_testFixture_for_a_Story()
        {
            var s = new Story("test", "summary", new Scenario[] { });
            Assembly a = BuildTestAssembly(s);
            a.GetTypes().Length.ShouldEqual(1);
        }

        private Assembly BuildTestAssembly(Story story)
        {
            var generator = new NUnitFixtureGenerator(new ScenarioPreprocessor(),  FakeMethodGenerator());
            var code = generator.GenerateFixture(story, GetContext());

            return CreateTestAssembly(code);
        }

        private static Assembly CreateTestAssembly(string code)
        {
            const string header = "using System;\r\nusing NUnit.Framework;";
            return TestHelper.CreateAssembly(header + "\r\n" + code);
        }

        private NUnitTestMethodGenerator FakeMethodGenerator()
        {
            var gen = Fake<NUnitTestMethodGenerator>();
            var testName = "Test" + Guid.NewGuid().ToString().Replace("-", "");
            string body = "\r\n[Test] public void " + testName + "() {}";
            gen.Stub(x=>x.GetTestFromScenario(null, null))
                .IgnoreArguments()
                .Return(new NUnitTest(testName, body, new TestContextField[0]));

            return gen;
        }


        private StoryContext GetContext()
        {
            return new StoryContext(new[] { typeof(TestContext) });
        }
    }
}