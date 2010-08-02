using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class LineNotInterpretedEvent
    {
        public Scenario Scenario;
        public string Line;
        public string Suggestion;
    }
}