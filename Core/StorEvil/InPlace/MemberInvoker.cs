using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StorEvil.InPlace
{
    public class MemberInvoker
    {
        public static event MemberInvokedHandler OnMemberInvoked;
        public object InvokeMember(MemberInfo info, IEnumerable<object> parameters, object context)
        {
            RaiseInvocationEvent(info, parameters, context);

            if (info.MemberType == MemberTypes.Method)
            {
                var methodParameters = ConvertParameters(info as MethodInfo, parameters);
                if (IsExtensionMethod(info))
                {
                   
                    methodParameters = new[] { context }.Concat(methodParameters.ToArray());
                    //context = null;
                }

                
                return  (info as MethodInfo).Invoke(context, methodParameters.ToArray());
            }

            if (info.MemberType == MemberTypes.Property)
                return (info as PropertyInfo).GetValue(context, null);
            
            if (info.MemberType == MemberTypes.Field)
                return (info as FieldInfo).GetValue(context);
            
            return null;
        }

        private void RaiseInvocationEvent(MemberInfo info, IEnumerable<object> parameters, object context)
        {
            if (OnMemberInvoked == null)
                return;
            
            OnMemberInvoked(this, new MemberInvokedHandlerArgs {Member = info, Parameters = parameters.ToArray(), Context = context});
        }

        private IEnumerable<object> ConvertParameters(MethodInfo info, IEnumerable<object> parameters)
        {
            ParameterInfo[] infos = info.GetParameters();
            for (int i = 0; i < parameters.Count(); i++)
            {
                var val = parameters.ElementAt(i);

                var paramType = infos[i].ParameterType;

                yield return ConvertParameter(paramType, val);
            }
        }

        private static object ConvertParameter(Type type, object val)
        {
            return Convert.ChangeType(val, type);
        }
        private static bool IsExtensionMethod(MemberInfo info)
        {
            // TODO: there must be a better way
            Type type = info.DeclaringType;
            return type.IsAbstract;
        }
    }

    public delegate void MemberInvokedHandler(object sender, MemberInvokedHandlerArgs args);

    public class MemberInvokedHandlerArgs
    {
        public MemberInfo Member;
        public object[] Parameters;
        public object Context;
    }
}