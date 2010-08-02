using System;

namespace StorEvil.Events
{
    [Serializable]
    public class LineExecuted
    {
        public string Line;
        public ExecutionStatus Status;

        public string SuccessPart = "";
        public string FailedPart = "";
        public string Message = "";
        public string Suggestion = "";
    }
}