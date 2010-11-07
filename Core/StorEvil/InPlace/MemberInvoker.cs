using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace StorEvil.InPlace
{
    public class MemberInvoker
    {       
        public object InvokeMember(MemberInfo info, IEnumerable<object> parameters, object context)
        {            
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
            if (info is MethodInfo)
            {
                return info.GetCustomAttributes(typeof (ExtensionAttribute), true).Any()
                       && info.DeclaringType.GetCustomAttributes(typeof (ExtensionAttribute), true).Any();
            }

            return false;
        }
    }

   
}