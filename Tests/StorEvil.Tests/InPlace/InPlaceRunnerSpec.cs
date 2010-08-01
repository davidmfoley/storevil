using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Interpreter;
using StorEvil.NUnit;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    public class InPlaceRunnerSpec<T>
    {
        protected IResultListener ResultListener;
        protected StoryContext Context;
        private CapturingEventBus FakeEventBus;

        protected void RunStory(Story story)
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();
            new ExtensionMethodHandler().AddAssembly(typeof(TestExtensionMethods).Assembly);

            Context = new StoryContext(new FakeSessionContext(), typeof (T));
            
            if (UseCompilingRunner())
            {
                RunInCompilingRunner(story);
            }
            else
            {
                GetRunner().HandleStory(story);
            }                     
        }

        private void RunInCompilingRunner(Story story)
        {
            var preprocessor = new ScenarioPreprocessor();

            var configSettings = new ConfigSettings
                                     {
                                         AssemblyLocations = new[]
                                                                 {
                                                                     GetType().Assembly.Location,
                                                                     typeof (TestExtensionMethods).Assembly.Location,
                                                                     typeof (Story).Assembly.Location
                                                                 }
                                     };

            var remoteHandlerFactory = new TestRemoteHandlerFactory(new AssemblyGenerator(), configSettings, new Filesystem() );
                                     
            var runner = new InPlaceCompilingStoryRunner(
                remoteHandlerFactory,                                                
                ResultListener, 
                preprocessor,                                               
                new IncludeAllFilter(), 
                new FakeSessionContext(Context), MockRepository.GenerateStub<IEventBus>());
            runner.HandleStory(story);
            runner.Finished();
        }

        private InPlaceStoryRunner GetRunner()
        {
            FakeEventBus = new CapturingEventBus();
            var scenarioInterpreter = new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()), MockRepository.GenerateStub<IAmbiguousMatchResolver>());
            return new InPlaceStoryRunner(ResultListener, new ScenarioPreprocessor(),
                                          scenarioInterpreter,
                                          new IncludeAllFilter(), new FakeSessionContext(Context), FakeEventBus);
        }

        public class CapturingEventBus : IEventBus
        {
            public List<object> CaughtEvents;

            public void Raise<T1>(T1 e)
            {
                CaughtEvents.Add(e);
            }

            public void Register(object handler)
            {
               
            }
        }

        private bool UseCompilingRunner()
        {
            if (null != this as UsingCompiledRunner)
                return true;

            if (null == this as UsingNonCompiledRunner)
                throw new ApplicationException(
                    "In place specs must implement UsingCompiledRunner or Using NonCompiledRunner");

            return false;
        }

        protected argT Any<argT>()
        {
            return Arg<argT>.Is.Anything;
        }

        protected static Scenario BuildScenario(string name, params string[] lines)
        {
            int lineNumber = 0;
            return new Scenario(name, lines.Select(line=> new ScenarioLine {Text = line, LineNumber = ++lineNumber}).ToArray());
        }

        protected void AssertLineSuccess(string expectedLine)
        {
            ResultListener.AssertWasCalled(x => x.Success(Any<Scenario>(), Arg<string>.Is.Equal(expectedLine)));
        }

        protected void AssertLineSuccess(string expectedLine, int count)
        {
            var args = ResultListener.GetArgumentsForCallsMadeOn(x =>
                                                          {
                                                              x.Success(Any<Scenario>(),
                                                                        Arg<string>.Is.Equal(expectedLine));
                                                          });
            args.Where(a=> (string)a[1] == expectedLine).Count().ShouldBe(count);
        }

        protected void AssertScenarioSuccess()
        {
            var args = ResultListener.GetArgumentsForCallsMadeOn(x => x.ScenarioSucceeded(Any<Scenario>()));
            args.Count().ShouldBeGreaterThan(0);
        }

        protected void AssertNoFailures()
        {
            ResultListener.AssertWasNotCalled(x => x.ScenarioFailed(Any<ScenarioFailureInfo>()));
        }

        protected void AssertAllScenariosSucceeded()
        {
            var args = ResultListener.GetArgumentsForCallsMadeOn(x => x.ScenarioFailed(Any<ScenarioFailureInfo>()));
            if (args.Count > 0)
            {
                var message = string.Join("\r\n", args.Select(a => ((ScenarioFailureInfo)a[0]).Message).ToArray());

                Assert.Fail(message);
            }
        }

        protected void AssertScenarioSuccessWithName(string name)
        {

            ResultListener.AssertWasCalled(x => x.ScenarioSucceeded(Arg<Scenario>.Matches(s => s.Name == name)));
        }
    }

    internal class TestRemoteHandlerFactory : RemoteHandlerFactory
    {
        public TestRemoteHandlerFactory(AssemblyGenerator assemblyGenerator, 
            ConfigSettings configSettings, Filesystem filesystem) : base(assemblyGenerator,configSettings,filesystem)
        {}
        public override IRemoteStoryHandler GetHandler(Story story, System.Collections.Generic.IEnumerable<Scenario> scenarios, IResultListener listener)
        {
            var handler = base.GetHandler(story, scenarios, listener) as RemoteStoryHandler;
            handler.InTest = true;

            return handler;
        } 
    }

    public class LocalStoryHandler : IRemoteStoryHandler
    {
        private readonly IStoryHandler _handler;

        public LocalStoryHandler(IStoryHandler handler)
        {
            _handler = handler;
        }

        public void Dispose()
        {
          
        }

        public IStoryHandler Handler
        {
            get { return _handler; }
        }
    }

    internal interface UsingCompiledRunner
    {
    }

    internal interface UsingNonCompiledRunner
    {
    }

    [Context]
    public class InPlaceRunnerTestContext
    {
        public static bool WhenSomeActionCalled;
        public static int? RegexMatchParamValue;
        public static bool WhenSomeOtherActionCalled;
        public static string MultiWordParam;

        public InPlaceRunnerTestContext()
        {
            WhenSomeActionCalled = false;
            RegexMatchParamValue = null;
            MultiWordParam = null;
        }

        public void WhenSomeAction()
        {
            WhenSomeActionCalled = true;
        }

        public bool then_some_action_was_called()
        {
            return WhenSomeActionCalled;
        }

        public void WhenSomeOtherAction()
        {
            WhenSomeOtherActionCalled = true;
        }

        [ContextRegex("^Matches a regex with ([0-9]+)")]
        public void RegexMatchWithParam(int param)
        {
            RegexMatchParamValue = param;
        }

        public int then_regex_match_param_value()
        {
            return RegexMatchParamValue ?? 0;
        }

        public void Pending_scenario_step()
        {
           ScenarioStatus.Pending();
        }

        public void test_param_with_multiple_words([MultipleWords] string param)
        {
            MultiWordParam = param;
        }

        public void multi_word_param_should_be(string expected)
        {
            MultiWordParam.ShouldBe(expected);
        }

        public InPlaceTestSubContext SubContext()
        {
            return new InPlaceTestSubContext();
        }

        public void WhenSomeFailingAction()
        {
            throw new Exception("test exception");
        }
    }

    public class InPlaceTestSubContext
    {
        public object Property
        {
            get { return true; }
        }
    }
}