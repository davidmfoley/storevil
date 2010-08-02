using System;
using System.Reflection;

namespace StorEvil.Events
{
    [Serializable]
    public class MatchFoundEvent
    {
        public MemberInfo Member;      
    }
}