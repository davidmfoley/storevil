using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.NUnit;
using StorEvil.Parsing;
using StorEvil.ResultListeners;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{



    [TestFixture]
    public class Inplace_assembly_generation
    {
        private AssemblyGenerator Generator;
        private string GeneratedAssemblyPath;
        private IScenario[] _scenarios;

        [SetUp]
        public void SetupContext()
        {
            Generator = new AssemblyGenerator(new ScenarioPreprocessor());
            _scenarios = new IScenario[]
                             {
                                 TestHelper.BuildScenario("foo", "When I do seomthing",
                                                          "something else should happen")
                             };
            GeneratedAssemblyPath = Generator.GenerateAssembly(new Story("foo", "bar", _scenarios),
                                                           new[] {this.GetType().Assembly.Location});
        }

        [Test]
        public void Should_exist()
        {
            File.Exists(GeneratedAssemblyPath).ShouldBe(true);
        }



        [Test]
        public void Should_be_able_to_instantiate()
        {
            var handle = Activator.CreateInstanceFrom(
                GeneratedAssemblyPath,
                "StorEvilTestAssembly.StorEvilDriver");

            var driver = handle.Unwrap() as IStoryHandler;

            driver.ShouldNotBeNull();

        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            File.Delete(GeneratedAssemblyPath);
        }
    }

    public class InPlaceCompilingRunnerSpec<T>
    {
        protected IResultListener ResultListener;
        protected StoryContext Context;
        protected InPlaceCompilingStoryRunner Runner;

        protected void RunStory(Story story)
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();
            new ExtensionMethodHandler().AddAssembly(typeof (TestExtensionMethods).Assembly);

            Context = new StoryContext(typeof (T));
            Runner = new InPlaceCompilingStoryRunner(ResultListener, new ScenarioPreprocessor(),
                                                     new ScenarioInterpreter(
                                                         new InterpreterForTypeFactory(new ExtensionMethodHandler())),
                                                     new IncludeAllFilter(), new StoryContextFactory());
            Runner.HandleStory(story);
        }

        protected argT Any<argT>()
        {
            return Arg<argT>.Is.Anything;
        }

        protected static Scenario BuildScenario(string name, params string[] lines)
        {
            return new Scenario("test", lines.Select(line => new ScenarioLine {Text = line}));
        }
    }
}