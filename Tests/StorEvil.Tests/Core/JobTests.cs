using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Utility;

namespace StorEvil.Core
{
    [TestFixture]
    public class JobTests : TestBase
    {
        [Test]
        public void Properties_Should_Match_Constructor_Parameters()
        {
            var storyProvider = Fake<IStoryProvider>();
            var testStoryHandler = Fake<IStoryHandler>();

            var job = new StorEvilJob(storyProvider, testStoryHandler);

            job.StoryProvider
                .ShouldEqual(storyProvider);

            job.Handler
                .ShouldEqual(testStoryHandler);
        }

        [Test]
        public void Invokes_Handler_For_Single_Story_And_Context()
        {
            var story = new Story("test context", "summary", new List<IScenario>());

            var job = GetJobWithMockDependencies();

            job.StoryProvider.Stub(x => x.GetStories()).Return(new[] {story});
            job.Handler.Stub(x => x.GetResult()).Return(new JobResult());
            job.Run();
            job.Handler.AssertWasCalled(x => x.HandleStories( new [] {story}));
        }

        [Test]
        public void Notifies_Handler_When_Finished()
        {
            var job = GetJobWithMockDependencies();
            job.Handler.Stub(x => x.GetResult()).Return(new JobResult());
            job.StoryProvider.Stub(x => x.GetStories())
                .Return(new[] {new Story("test context", "summary", new List<IScenario>())});

            job.Run();

            job.Handler.AssertWasCalled(x => x.Finished());
        }

        private StorEvilJob GetJobWithMockDependencies()
        {
            return new StorEvilJob(
                Fake<IStoryProvider>(),
                Fake<IStoryHandler>());
        }
    }

    [TestFixture]
    public class Executing_a_remote_debugging_job
    {
        
    }
}