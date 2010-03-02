using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;

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
            
            var inPlaceRunner = new InPlaceRunner(ResultListener, new ScenarioPreprocessor());
            inPlaceRunner.Finished();
        }
        [Test]
        public void notifies_result_listener()
        {

            ResultListener.AssertWasCalled(x => x.Finished());
        }
    }
}