using StorEvil.Core;

namespace StorEvil.Nunit
{
    public class FixtureGenerationStoryHandler : IStoryHandler
    {
        private readonly IFixtureGenerator FixtureGenerator;
        private readonly ITestFixtureWriter TestFixtureWriter;

        public FixtureGenerationStoryHandler(IFixtureGenerator fixtureGenerator,
                                             ITestFixtureWriter testFixtureWriter)
        {
            FixtureGenerator = fixtureGenerator;
            TestFixtureWriter = testFixtureWriter;
        }

        public void HandleStory(Story story, StoryContext context)
        {
            var sourceCode = FixtureGenerator.GenerateFixture(story, context);

            TestFixtureWriter.WriteFixture(story.Id, sourceCode);
        }

        public void Finished()
        {
            TestFixtureWriter.Finished();
        }
    }
}