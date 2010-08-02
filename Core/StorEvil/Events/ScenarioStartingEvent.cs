using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class ScenarioStartingEvent
    {
        public IScenario Scenario;
    }
}