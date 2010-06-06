using System;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.NUnit
{
    public class FakeSessionContext  : ISessionContext
    {
        private StoryContext _context;

        public FakeSessionContext()
        {
        }

        public FakeSessionContext(StoryContext context)
        {
            _context = context;
        }

        public StoryContext GetContextForStory(Story story)
        {
            return _context ?? (_context = new StoryContext(this));
        }

        public void SetContext(object context)
        {
            
        }
    }
}