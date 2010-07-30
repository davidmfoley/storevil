using System;
using System.Linq;

namespace StorEvil.Context
{
    public static class ContextAttributeHelper
    {

        public static ContextInfo GetContextInfo(this Type contextType)
        {
            var customAttrs = contextType.GetCustomAttributes(true);
            var contextAttribute =
                customAttrs.FirstOrDefault(x => x.GetType().FullName == typeof(ContextAttribute).FullName);

            if (contextAttribute == null)
                return new ContextInfo { Lifetime = ContextLifetime.Scenario };

            var lifetime = contextAttribute.GetType().GetProperty("Lifetime").GetValue(contextAttribute, null);

            return new ContextInfo { Lifetime = (ContextLifetime)Enum.Parse(typeof(ContextLifetime), lifetime.ToString(), true) };
        }
    }
}