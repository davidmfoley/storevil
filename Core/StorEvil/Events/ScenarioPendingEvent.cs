using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class ScenarioPendingEvent
    {
        public Scenario Scenario;
        public string Line;
    }
}