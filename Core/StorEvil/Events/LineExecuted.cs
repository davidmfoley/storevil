using System;
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

    [Serializable]
    public class LinePassed
    {
        public string Line;
        public Scenario Scenario;
    }

    [Serializable]
    public class LineFailed
    {
        public string Line;
       
        public string SuccessPart = "";
        public string FailedPart = "";
        public string ExceptionInfo = "";
        
        public Scenario Scenario;
    }

    [Serializable]
    public class LinePending
    {
        public string Line;       
        public string Suggestion = "";
        public Scenario Scenario;
    }
}