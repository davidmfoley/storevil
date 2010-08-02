using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class LineInterpretedEvent
    {
        public Scenario Scenario;
        public string Line;
    }
}