using System.Collections.Generic;

namespace StorEvil.Context
{
    /// <summary>
    /// This is returned when the terms in the scenario line match exactly (word-for-word) with
    /// the name and/or parameters of a member. 
    /// 
    /// For example:
    /// given the following function:
    ///      When_I_Deposit(decimal depositAmount) {}
    /// and the following line in a scenario:
    ///      when I deposit 300
    /// 
    ///... an exact match would be returned 
    ///
    /// </summary>
    public class ExactMatch : NameMatch
    {
        public ExactMatch(Dictionary<string, object> paramValues, string matchedText) : base(paramValues, matchedText)
        {
        }
    }
}