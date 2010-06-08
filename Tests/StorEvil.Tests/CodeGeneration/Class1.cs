using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Utility;

namespace StorEvil.CodeGeneration
{
    public abstract class Generating_Code
    {
        private CustomToolCodeGenerator Generator;
        private string Result;
        private Assembly CompiledAssembly;
        private object Instance;
        private Type TestFixtureType;

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

            Scenario scenario = new Scenario("scenario-id", "scenario-name", body);
            return new Story("foo", "bar",  new IScenario[] {scenario});
        }

        private ScenarioLine[] GetScenarioBody(params string[] body)
        {

            return body.Select((x, i) => new ScenarioLine {LineNumber = i, Text = x}).ToArray();
        }
    }
}
