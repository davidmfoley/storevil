using NUnit.Framework;
using System.Collections.Generic;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Nunit;
using StorEvil.Utility;

namespace StorEvil
{
    [TestFixture]
    public class JobTests : TestBase
    {
        [Test]
        public void Properties_Should_Match_Constructor_Parameters()
        {
            var storyProvider = Fake<IStoryProvider>();
            var storyToContextMapper = Fake<IStoryToContextMapper>();
            var testStoryHandler = Fake<IStoryHandler>();

            var job = new StorEvilJob(storyProvider, storyToContextMapper, testStoryHandler);

            job.StoryProvider
                .ShouldEqual(storyProvider);

            job.StoryToContextMapper
                .ShouldEqual(storyToContextMapper);

            job.Handler
                .ShouldEqual(testStoryHandler);

        }

        [Test]
        public void Invokes_Handler_For_Single_Story_And_Context()
        {
            var story = new Story("test context", "summary", new List<IScenario>());
            var contextType = new StoryContext(typeof(object));

            var job = GetJobWithMockDependencies();


            job.StoryProvider.Stub(x => x.GetStories()).Return(new[] { story });

            job.StoryToContextMapper.Stub(x => x.GetContextForStory(story))
              .Return(contextType);

            job.Run();
            job.Handler.AssertWasCalled(x => x.HandleStory(story, contextType));
        }

        [Test]
        public void Notifies_Handler_When_Finished()
        {
            var job = GetJobWithMockDependencies();

            job.StoryProvider.Stub(x => x.GetStories())
                .Return(new[] { new Story("test context", "summary", new List<IScenario>()) });

            job.Run();

            job.Handler.AssertWasCalled(x => x.Finished());
        }

        private StorEvilJob GetJobWithMockDependencies()
        {
            return new StorEvilJob(
                Fake<IStoryProvider>(),
                Fake<IStoryToContextMapper>(),
                Fake<IStoryHandler>());
        }
    }


}