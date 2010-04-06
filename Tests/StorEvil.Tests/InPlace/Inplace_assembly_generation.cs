using System;
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
        private Assembly GeneratedAssembly;
        private IScenario[] _scenarios;

        [SetUp]
        public void SetupContext()
        {
            Generator = new AssemblyGenerator();
            _scenarios = new IScenario[]
                             {
                                 TestHelper.BuildScenario("foo", "When I do seomthing",
                                                          "something else should happen")
                             };
            GeneratedAssembly = Generator.GenerateAssembly(new Story("foo", "bar", _scenarios),
                                                           _scenarios.Cast<Scenario>().ToArray());
        }

        [Test]
        public void Should_compile()
        {
            GeneratedAssembly.ShouldNotBeNull();
        }

        [Test]
        public void Should_have_a_driver_class()
        {
            GetDriverType().ShouldNotBeNull();
        }

        private Type GetDriverType()
        {
            return GeneratedAssembly.GetType("StorEvilTestAssembly.StorEvilDriver");
        }

        [Test]
        public void Driver_class_exposes_an_Execute_method()
        {
            GetDriverType().GetMethod("Execute").ShouldNotBeNull();
        }

        [Test]
        public void Should_be_able_to_execute()
        {
            IResultListener listener = new ConsoleResultListener();
            MemberInvoker memberInvoker = new MemberInvoker();
            ScenarioInterpreter scenarioInterpreter =
                new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()));
            var scenarioPreprocessor = new ScenarioPreprocessor();
            var scenarios = _scenarios.SelectMany(x => scenarioPreprocessor.Preprocess(x)).ToArray();

            var driver = Activator.CreateInstance(GetDriverType(),
                                                  listener,
                                                  memberInvoker,
                                                  scenarioInterpreter,
                                                  scenarios
                );

            GetDriverType().GetMethod("Execute").Invoke(driver, new[] {new StoryContext()});
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