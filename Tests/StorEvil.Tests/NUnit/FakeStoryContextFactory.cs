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
            if (_context == null)
                _context = new StoryContext(this);
            return  _context;
        }

        public void SetContext(object context)
        {
            
        }
    }
}