using System;
using System.Collections.Generic;
using System.Reflection;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.NUnit
{
    public class FakeSessionContext  : ISessionContext
    {
        private StoryContext _context;
        public bool WasDisposed { get; set; }

        public FakeSessionContext()
        {
        }

        public FakeSessionContext(StoryContext context)
        {
            _context = context;
        }

        public StoryContext GetContextForStory()
        {
            return _context ?? (_context = new StoryContext(this));
        }

        public void SetContext(object context)
        {
            
        }

        public IEnumerable<Assembly> GetAllAssemblies()
        {
            return new Assembly[0];
        }

        public void Dispose()
        {
            WasDisposed = true;
        }
    }
}