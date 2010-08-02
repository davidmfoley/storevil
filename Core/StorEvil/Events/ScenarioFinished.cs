using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class ScenarioFinished
    {
        public Scenario Scenario;
        public ExecutionStatus Status;
    }

    public enum ExecutionStatus
    {
        Passed,
        Failed,
        Pending,
        CouldNotInterpret
    }
}