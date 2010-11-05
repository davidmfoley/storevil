using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
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
            StubHandleStoriesReturn(job, new JobResult {});
            job.Run();
            job.Handler.AssertWasCalled(x => x.HandleStories( new [] {story}));
        }

        [Test]
        public void returns_failure_count()
        {
            var job = GetJobWithMockDependencies();
            StubHandleStoriesReturn(job, new JobResult{Failed = 42});
            job.StoryProvider.Stub(x => x.GetStories())
                .Return(new[] {new Story("test context", "summary", new List<IScenario>())});

            job.Run().ShouldBe(42);

        }

        private void StubHandleStoriesReturn(StorEvilJob job, JobResult result)
        {
            job.Handler.Stub(x => x.HandleStories(Arg<IEnumerable<Story>>.Is.Anything)).Return(result);
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