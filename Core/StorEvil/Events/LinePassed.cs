using System;
using StorEvil.Core;

namespace StorEvil.Events
{
    [Serializable]
    public class LinePassed
    {
        public string Line;
        public Scenario Scenario;
    }
}