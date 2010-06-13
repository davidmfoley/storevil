using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Utility;

namespace StorEvil.CodeGeneration
{
    public abstract class Generating_Code
    {
        private CustomToolCodeGenerator Generator;
        protected string Result;
        private Assembly CompiledAssembly;
        private object Instance;
        protected Type TestFixtureType;

        [TestFixtureSetUp]
        public void SetUpContext()
        {
            Generator = new CustomToolCodeGenerator();
       
            Story story = GetStory();

            Result = Generator.Generate(story);

            CompiledAssembly = new CodeCompiler().CompileInMemory(Result, new[] { typeof(Scenario).Assembly, typeof(TestFixtureAttribute).Assembly });
            TestFixtureType = CompiledAssembly.GetTypes().First();
            Instance = Activator.CreateInstance(TestFixtureType);
        }

        protected abstract Story GetStory();

        [Test]
        public void Should_always_compile()
        {
           Instance.ShouldNotBeNull();
        }

        [Test]
        public void Fixture_should_have_TestFixture_attribute()
        {
            TestFixtureType.GetCustomAttributes(typeof(TestFixtureAttribute), true).Any().ShouldBe(true);            
        }
    }

    [TestFixture]
    public class for_an_empty_story : Generating_Code
    {
        protected  override Story GetStory()
        {
            return new Story("foo", "bar", new IScenario[0]);
        }
    }

    [TestFixture]
    public class for_a_story_with_one_scenario : Generating_Code
    {
        protected override Story GetStory()
        {
            ScenarioLine[] body = GetScenarioBody("when I do something", "then I should see something");

            Scenario scenario = new Scenario("scenario-id", "scenario name", body);
            return new Story("foo bar baz", "bar",  new IScenario[] {scenario});
        }

        private ScenarioLine[] GetScenarioBody(params string[] body)
        {

            return body.Select((x, i) => new ScenarioLine {LineNumber =  + 1, Text = x}).ToArray();
        }

        [Test]
        public void Should_have_a_single_test_method()
        {
            GetTestMethods().Count().ShouldEqual(1);
        }

        [Test]
        public void test_name_should_match_scenario_name()
        {
            GetTestMethods().First().Name.ShouldEqual("scenario_name");
        }

        [Test]
        public void should_set_class_name_based_on_id_for_now()
        {
            TestFixtureType.Name.ShouldEqual("foo_bar_baz");
        }


        private IEnumerable<MethodInfo> GetTestMethods()
        {
            return TestFixtureType.GetMethods()
                .Where(m => m.GetCustomAttributes(typeof (TestAttribute), true)
                    .Any());
        }
    }

    [TestFixture]
    public class TestSession_Behavior
    {       
        [Test]
        public void returns_same_context_for_multiple_calls()
        {
            var context = TestSession.SessionContext;
            Assert.That(context, Is.SameAs(TestSession.SessionContext));
        }

        [Test]
        public void returns_different_context_after_ShutDown()
        {
            var context = TestSession.SessionContext;
            TestSession.ShutDown();
            
            Assert.That(context, Is.Not.SameAs(TestSession.SessionContext));
        }
    }
}
