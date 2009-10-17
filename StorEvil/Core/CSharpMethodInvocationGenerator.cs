using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context;

namespace StorEvil.Core
{
    /// <summary>
    /// Handles mapping 
    /// 
    /// TODO: need to break out method mapping from code gen here
    /// </summary>
    public class CSharpMethodInvocationGenerator
    {
        private readonly ScenarioInterpreter _interpreter;

        public CSharpMethodInvocationGenerator(ScenarioInterpreter interpreter)
        {
            _interpreter = interpreter;
        }

        /// <summary>
        /// Maps a line of a scenario to invocations of methods on the test context object
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public string MapMethod(ScenarioContext scenarioContext, string line)
        {
            var matchingChain = _interpreter.GetChain(scenarioContext, line);

            if (matchingChain == null)
                return null;

            return ConvertInvocationChainToCSharpCode(matchingChain);
        }

        private static string ConvertInvocationChainToCSharpCode(InvocationChain matchingChain)
        {
            string code = "";
            foreach (Invocation invocation in matchingChain.Invocations)
                code += BuildInvocation(invocation.MemberInfo, invocation.ParamValues);

            return code;
        }

        private static string  BuildInvocation(MemberInfo memberInfo, IEnumerable<object> paramValues)
        {
            if (memberInfo is MethodInfo)
            {
                return BuildMethodInvocation((MethodInfo) memberInfo, paramValues);
            }    
            if (memberInfo is PropertyInfo)
            {
                return BuildPropertyInvocation((PropertyInfo) memberInfo);
            }
            if (memberInfo is FieldInfo)
            {
                return BuildFieldInvocation((FieldInfo) memberInfo);
            }

            return null;
        }

        private static string BuildFieldInvocation(FieldInfo info)
        {
            return "." + info.Name;
        }

        private static string BuildPropertyInvocation(PropertyInfo info)
        {
            return "." + info.Name;
        }

        private static string BuildMethodInvocation(MethodBase member, IEnumerable<object> paramValues)
        {
            var parameters = new string[paramValues.Count()];
            ParameterInfo[] infos = member.GetParameters();
            for (int i = 0; i < paramValues.Count(); i++)
            {
                
                parameters[i] = ParameterValueFormatter.GetParamString(infos[i].ParameterType,
                                                                       paramValues.ElementAt(i).ToString());
            }
            return string.Format(".{0}({1})", member.Name, String.Join(", ", parameters)); 
        }

        public CSharpMethodInvocationGenerator ForType(Type type)
        {
            return new CSharpMethodInvocationGenerator(_interpreter);
        }
    }
}