using System;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Context;
using StorEvil.Events;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.NUnit;
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
            var inPlaceRunner = new InPlaceCompilingStoryRunner(remoteHandlerFactory,new IncludeAllFilter(),  eventBus);
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
        private FakeSessionContext FakeSessionContext;

        [SetUp]
        public void SetupContext()
        {
            FakeEventBus = MockRepository.GenerateStub<IEventBus>();
            FakeSessionContext = new FakeSessionContext();
            var scenarioInterpreter = new ScenarioInterpreter(new InterpreterForTypeFactory(new AssemblyRegistry()), MockRepository.GenerateStub<IAmbiguousMatchResolver>(), new DefaultLanguageService());
            var inPlaceRunner = new InPlaceStoryRunner(scenarioInterpreter, new IncludeAllFilter(), FakeSessionContext, FakeEventBus);
            inPlaceRunner.Finished();
        }

        [Test]
        public void notifies_result_listener()
        {
            FakeEventBus.AssertWasCalled(x => x.Raise(Arg<SessionFinished>.Is.Anything));
        }

 
    }
}