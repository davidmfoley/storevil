using StorEvil.Context;
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

        public int HandleStory(Story story, StoryContext context)
        {
            var sourceCode = FixtureGenerator.GenerateFixture(story, context);

            TestFixtureWriter.WriteFixture(story.Id, sourceCode);
            return 0;
        }

        public void Finished()
        {
            TestFixtureWriter.Finished();
        }
    }
}