using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.InPlace_Compiled
{
    [TestFixture]
    public class When_finished_running_all_stories
    {
        private IResultListener ResultListener;

        [SetUp]
        public void SetupContext()
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();

            var inPlaceRunner = new InPlaceCompilingStoryRunner(ResultListener, new ScenarioPreprocessor(), new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler())), new IncludeAllFilter());
            inPlaceRunner.Finished();
        }

        [Test]
        public void notifies_result_listener()
        {
            ResultListener.AssertWasCalled(x => x.Finished());
        }
    }
}

namespace StorEvil.InPlace
{
    [TestFixture]
    public class When_finished_running_all_stories
    {
        private IResultListener ResultListener;

        [SetUp]
        public void SetupContext()
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();

            var inPlaceRunner = new InPlaceStoryRunner(ResultListener, new ScenarioPreprocessor(), new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler())), new IncludeAllFilter());
            inPlaceRunner.Finished();
        }

        [Test]
        public void notifies_result_listener()
        {
            ResultListener.AssertWasCalled(x => x.Finished());
        }
    }
}