using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class When_finished_running_all_stories
    {
        private IResultListener ResultListener;

        [SetUp]
        public void SetupContext()
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();
            var remoteHandlerFactory = MockRepository.GenerateStub<IRemoteHandlerFactory>();            

            var inPlaceRunner = new InPlaceCompilingStoryRunner(remoteHandlerFactory, ResultListener,new ScenarioPreprocessor(), new IncludeAllFilter(), new StoryContextFactory());
            inPlaceRunner.Finished();
        }

        [Test]
        public void notifies_result_listener()
        {
            ResultListener.AssertWasCalled(x => x.Finished());
        }
    }
}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class When_finished_running_all_stories
    {
        private IResultListener ResultListener;

        [SetUp]
        public void SetupContext()
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();

            var scenarioInterpreter = new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()), MockRepository.GenerateStub<IAmbiguousMatchResolver>());
            var inPlaceRunner = new InPlaceStoryRunner(ResultListener, new ScenarioPreprocessor(), scenarioInterpreter, new IncludeAllFilter(), new StoryContextFactory());
            inPlaceRunner.Finished();
        }

        [Test]
        public void notifies_result_listener()
        {
            ResultListener.AssertWasCalled(x => x.Finished());
        }
    }
}