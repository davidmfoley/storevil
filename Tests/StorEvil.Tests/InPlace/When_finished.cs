using System;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Events;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class When_finished_running_all_stories
    {
  
        [Test]
        public void notifies_result_listener()
        {
            var remoteHandlerFactory = MockRepository.GenerateStub<IRemoteHandlerFactory>();

            EventBus eventBus = new EventBus();
            var fakeHandler = new FinishedTestHandler();
            eventBus.Register(fakeHandler);
            var inPlaceRunner = new InPlaceCompilingStoryRunner(remoteHandlerFactory, new ScenarioPreprocessor(), new IncludeAllFilter(), new SessionContext(), eventBus);
            inPlaceRunner.Finished();
            fakeHandler.FinishedWasCalled.ShouldEqual(true);
        }

        public class FinishedTestHandler : IHandle<SessionFinished>
        {
            public bool FinishedWasCalled;
            public void Handle(SessionFinished eventToHandle)
            {
                FinishedWasCalled = true;
            }
        }

    }
}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class When_finished_running_all_stories
    {
        private IEventBus FakeEventBus;

        [SetUp]
        public void SetupContext()
        {
            FakeEventBus = MockRepository.GenerateStub<IEventBus>();

            var scenarioInterpreter = new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()), MockRepository.GenerateStub<IAmbiguousMatchResolver>());
            var inPlaceRunner = new InPlaceStoryRunner(new ScenarioPreprocessor(), scenarioInterpreter, new IncludeAllFilter(), new SessionContext(), FakeEventBus);
            inPlaceRunner.Finished();
        }

        [Test]
        public void notifies_result_listener()
        {
            FakeEventBus.AssertWasCalled(x => x.Raise(Arg<SessionFinished>.Is.Anything));
        }
    }
}