using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class ScenarioFailed
    {
        public Scenario Scenario;
        public string ExceptionInfo = "";
    }
}
