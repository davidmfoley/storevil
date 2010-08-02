using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class ScenarioSucceededEvent
    {
        public Scenario Scenario;
    }
}