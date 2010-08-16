using System;
using System.Collections.Generic;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.NUnit
{
    public class FixtureGenerationStoryHandler : IStoryHandler
    {
        private readonly IFixtureGenerator FixtureGenerator;
        private readonly ITestFixtureWriter TestFixtureWriter;
        private readonly ISessionContext _context;

        public FixtureGenerationStoryHandler(IFixtureGenerator fixtureGenerator,
                                             ITestFixtureWriter testFixtureWriter,
                                                ISessionContext context)
        {
            FixtureGenerator = fixtureGenerator;
            TestFixtureWriter = testFixtureWriter;
            _context = context;
        }

        public void HandleStory(Story story)
        {
            var sourceCode = FixtureGenerator.GenerateFixture(story, _context.GetContextForStory());

            TestFixtureWriter.WriteFixture(story.Id, sourceCode);
        }

        public void HandleStories(IEnumerable<Story> stories)
        {
            foreach (var story in stories)
            {
                HandleStory(story);
            }
        }

        public void Finished()
        {
            TestFixtureWriter.WriteSetUpAndTearDown(FixtureGenerator.GenerateSetupTearDown(_context));
            TestFixtureWriter.Finished();
        }

        public JobResult GetResult()
        {
            return new JobResult();
        }
    }
}