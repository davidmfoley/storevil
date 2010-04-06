using System;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.NUnit
{
    public class FakeStoryContextFactory  : IStoryContextFactory
    {
        private StoryContext _context;

        public FakeStoryContextFactory()
        {
        }

        public FakeStoryContextFactory(StoryContext context)
        {
            _context = context;
        }

        public StoryContext GetContextForStory(Story story)
        {
            return  _context ?? new StoryContext();
        }
    }
}