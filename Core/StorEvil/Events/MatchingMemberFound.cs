using System;
using System.Reflection;

namespace StorEvil.Events
{
    [Serializable]
    public class MatchingMemberFound
    {
        public MemberInfo Member;      
    }
}