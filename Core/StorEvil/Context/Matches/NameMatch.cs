using System.Collections.Generic;

namespace StorEvil.Context.Matches
{
    /// <summary>
    /// When we resolve a line of a scenario to a context object, 
    /// we return a subclass of this class for each match.
    /// </summary>
    public abstract class NameMatch
    {
        public Dictionary<string, object> ParamValues { get; set; }

        public string MatchedText { get; set; }

        protected NameMatch(Dictionary<string, object> paramValues, string matchedText)
        {
            ParamValues = paramValues;
            MatchedText = matchedText;
        }
    }
}