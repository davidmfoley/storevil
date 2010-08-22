using System;
using System.Reflection;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class LineExecuted
    {
        public string Line;
        public ExecutionStatus Status;

        public string SuccessPart = "";
        public string FailedPart = "";
        public string ExceptionInfo = "";
        public string Suggestion = "";
        public Scenario Scenario;       
    }
}