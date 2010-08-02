using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class ScenarioStarting
    {
        public IScenario Scenario;
    }
}