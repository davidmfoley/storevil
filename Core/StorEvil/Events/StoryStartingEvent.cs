using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class StoryStartingEvent
    {
        public Story Story;
    }
}