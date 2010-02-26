using NUnit.Framework;
using Rhino.Mocks;

namespace StorEvil.InPlace
{
    public class When_finished : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        [Test]
        public void notifies_result_listener()
        {
            Runner.Finished();
            ResultListener.AssertWasCalled(x => x.Finished());
        }
    }
}