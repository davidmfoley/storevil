using System;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.Nunit
{
    public class FixtureGenerationStoryHandler : IStoryHandler
    {
        private readonly IFixtureGenerator FixtureGenerator;
        private readonly ITestFixtureWriter TestFixtureWriter;
        private readonly IStoryContextFactory _contextFactory;

        public FixtureGenerationStoryHandler(IFixtureGenerator fixtureGenerator,
                                             ITestFixtureWriter testFixtureWriter,
                                                IStoryContextFactory contextFactory)
        {
            FixtureGenerator = fixtureGenerator;
            TestFixtureWriter = testFixtureWriter;
            _contextFactory = contextFactory;
        }

        public void HandleStory(Story story)
        {
            var sourceCode = FixtureGenerator.GenerateFixture(story, _contextFactory.GetContextForStory(story));

            TestFixtureWriter.WriteFixture(story.Id, sourceCode);
        }

        public void Finished()
        {
            TestFixtureWriter.Finished();
        }

        public StorEvilResult GetResult()
        {
            return new StorEvilResult();
        }
    }
}