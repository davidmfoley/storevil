using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context;
using StorEvil.Interpreter;

namespace StorEvil.Nunit
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

        public LineInfo MapMethod(ScenarioContext scenarioContext, string line)
        {
            var matchingChain = _interpreter.GetChain(scenarioContext, line);

            if (matchingChain == null)
                return null;

            return new LineInfo
                       {
                           Code = ConvertInvocationChainToCSharpCode(matchingChain),
                           Namespaces = GetNamespaces(matchingChain),
                           ImplementingType = matchingChain.Invocations.First().MemberInfo.DeclaringType
                       };
        }

        private IEnumerable<string> GetNamespaces(InvocationChain matchingChain)
        {
            return matchingChain.Invocations.Select(x => x.MemberInfo.DeclaringType.Namespace).Distinct();
        }

        private static string ConvertInvocationChainToCSharpCode(InvocationChain matchingChain)
        {
            Func<string, Invocation, string> aggregator =
                (codeSoFar, invocation) => codeSoFar + BuildInvocation(invocation.MemberInfo, invocation.RawParamValues);

            return matchingChain.Invocations
                .Aggregate("", aggregator);
        }

        private static string BuildInvocation(MemberInfo memberInfo, IEnumerable<string> paramValues)
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

        private static string BuildMethodInvocation(MethodBase member, IEnumerable<string> paramValues)
        {
            var parameters = new string[paramValues.Count()];
            ParameterInfo[] infos = member.GetParameters();
            for (int i = 0; i < paramValues.Count(); i++)
            {
                parameters[i] = ParameterValueFormatter.GetParamString(infos[i].ParameterType,
                                                                       paramValues.ElementAt(i));
            }

            return string.Format(".{0}({1})", member.Name, String.Join(", ", parameters));
        }

        public CSharpMethodInvocationGenerator ForType(Type type)
        {
            return new CSharpMethodInvocationGenerator(_interpreter);
        }
    }

    public class LineInfo
    {
        public string Code;
        public IEnumerable<string> Namespaces;
        public Type ImplementingType;
    }
}