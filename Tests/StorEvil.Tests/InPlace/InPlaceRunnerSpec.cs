using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Infrastructure;
using StorEvil.Interpreter;
using StorEvil.NUnit;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    public class InPlaceRunnerSpec<T>
    {
        protected StoryContext Context;
        protected CapturingEventBus FakeEventBus;

        protected void RunStory(Story story)
        {
            FakeEventBus = new CapturingEventBus();
            //new ExtensionMethodHandler(new AssemblyRegistry(new[] { typeof(TestExtensionMethods).Assembly}));

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

        protected void AssertWasRaised<TEvent>()
        {
            FakeEventBus.CaughtEvents.OfType<TEvent>().Any().ShouldEqual(true);
        }

        protected void AssertWasRaised<TEvent>(Predicate<TEvent> matching)
        {
            FakeEventBus.CaughtEvents.OfType<TEvent>().Any(x=>matching(x)).ShouldEqual(true);
        }
        private void RunInCompilingRunner(Story story)
        {
           
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
                new IncludeAllFilter(), 
                FakeEventBus);

            runner.HandleStories(new[] { story});
            runner.Finished();
        }

        private InPlaceStoryRunner GetRunner()
        {
            FakeEventBus = new CapturingEventBus();
            var assemblyRegistry = new AssemblyRegistry(new[] { typeof(TestExtensionMethods).Assembly });
            var scenarioInterpreter = new ScenarioInterpreter(new InterpreterForTypeFactory(assemblyRegistry), MockRepository.GenerateStub<IAmbiguousMatchResolver>(), new DefaultLanguageService());
            return new InPlaceStoryRunner(scenarioInterpreter,
                                          new IncludeAllFilter(), new FakeSessionContext(Context), FakeEventBus);
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
            GetSuccessfulInvocationCount(expectedLine).ShouldBeGreaterThan(0);
        }

        private int GetSuccessfulInvocationCount(string expectedLine)
        {
            var interpretedEvents = FakeEventBus.CaughtEvents.OfType<LinePassed>();
            return interpretedEvents.Count(ev => ev.Line == expectedLine);
        }

        protected void AssertLineSuccess(string expectedLine, int count)
        {
            GetSuccessfulInvocationCount(expectedLine).ShouldEqual(count);
        }

        protected void AssertScenarioSuccess()
        {
            AssertEventRaised<ScenarioPassed>();            
        }

        protected void AssertAllScenariosSucceeded()
        {
            FakeEventBus.CaughtEvents.OfType<ScenarioFailed>().Any().ShouldBe(false);
        }

        protected void AssertScenarioSuccessWithName(string name)
        {
            AssertEventRaised<ScenarioPassed>(x => x.Scenario.Name == name);                   
        }

        protected void AssertEventRaised<TEvent>(Predicate<TEvent> matching)
        {
            FakeEventBus.CaughtEvents.OfType<TEvent>().Any(x => matching(x)).ShouldEqual(true);
        }

        protected void AssertEventRaised<TEvent>()
        {
            AssertEventRaised<TEvent>(x => true);
        }

        protected void AssertEventNotRaised<TEvent>(Predicate<TEvent> matching)
        {
            FakeEventBus.CaughtEvents.OfType<TEvent>().Any(x => matching(x)).ShouldEqual(false);
        }

        protected void AssertEventNotRaised<TEvent>()
        {
            AssertEventNotRaised<TEvent>(x => true);
        }
    }

    internal class TestRemoteHandlerFactory : RemoteHandlerFactory
    {
        public TestRemoteHandlerFactory(AssemblyGenerator assemblyGenerator, 
            ConfigSettings configSettings, Filesystem filesystem) : base(assemblyGenerator, new AssemblyRegistry(configSettings.AssemblyLocations), filesystem)
        {}
        public override IRemoteStoryHandler GetHandler(IEnumerable<Story> stories, IEventBus eventBus)
        {
            var handler = base.GetHandler(stories, eventBus) as RemoteStoryHandler;
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

        public JobResult HandleStories(IEnumerable<Story> stories)
        {
            return Handler.HandleStories(stories);           
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