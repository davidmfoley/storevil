using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class ScenarioFailedEvent
    {
        public Scenario Scenario;
        public string Line;
        public string SuccessPart;
        public string FailedPart;
        public string Message;
    }
}