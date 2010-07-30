using System.Reflection;

namespace StorEvil.InPlace
{

    public delegate void MatchFoundHandler(object sender, MatchFoundHandlerArgs args);

    public class MatchFoundHandlerArgs
    {
        public MemberInfo Member;
    }

    public class StorEvilEvents
    {
        public static event MatchFoundHandler OnMatchFound;
        public static void RaiseMatchFound(object sender, MemberInfo info)        
        {
            if (OnMatchFound == null)
                return;

            OnMatchFound(sender, new MatchFoundHandlerArgs { Member = info});

        }
    }
}