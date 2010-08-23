using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class LineFailed
    {
        public string Line;
       
        public string SuccessPart = "";
        public string FailedPart = "";
        public string ExceptionInfo = "";
        
        public Scenario Scenario;
    }
}