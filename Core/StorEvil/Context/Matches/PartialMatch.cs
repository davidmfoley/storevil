using System;
using System.Collections.Generic;
using System.Reflection;

namespace StorEvil.Context.Matches
{
    /// <summary>
    /// Represents a partial match, which occurs when all of a member's words match 
    /// but there are other words left over... for example:
    /// if the context function is defined as
    ///     UserOptions Given_A_User_Named(string name)
    /// it would return a partial match for the following line:
    ///     Given a user named Dave who is an administrator
    /// 
    /// Note that the member must not return void for patial maches to work.
    /// The type of the member (in the above case UserOptions) is then used to resolve 
    /// the rest of the line (would need to implement a member Who_Is_An_Administrator())
    /// </summary>
    public class PartialMatch : NameMatch
    {
        public string RemainingText { get; set; }
        public MemberInfo MemberInfo;

        public PartialMatch(MemberInfo memberInfo, Dictionary<string, object> paramValues, string matchedText,
                            string remainingText) : base(paramValues, matchedText)
        {
            RemainingText = remainingText;
            MemberInfo = memberInfo;

            if (MemberInfo is MethodInfo)
                TerminatingType = ((MethodInfo) MemberInfo).ReturnType;
            else if (MemberInfo is PropertyInfo)
                TerminatingType = ((PropertyInfo) MemberInfo).PropertyType;
            else if (MemberInfo is FieldInfo)
                TerminatingType = ((FieldInfo) MemberInfo).FieldType;
        }

        /// <summary>
        /// The type returned, used to chain member invocations.
        /// </summary>
        public Type TerminatingType { get; private set; }
    }
}