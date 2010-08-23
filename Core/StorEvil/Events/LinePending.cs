using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class LinePending
    {
        public string Line;       
        public string Suggestion = "";
        public Scenario Scenario;
    }
}