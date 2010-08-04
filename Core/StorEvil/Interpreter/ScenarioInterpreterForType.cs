using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matchers;
using StorEvil.Context.Matches;
using StorEvil.Interpreter.ParameterConverters;

namespace StorEvil.Interpreter
{
    public class ScenarioInterpreterForType
    {
        private readonly Type _type;
        private readonly IInterpreterForTypeFactory _factory;
        private readonly List<IMemberMatcher> _memberMatchers = new List<IMemberMatcher>();
        private static readonly ParameterConverter _parameterConverter = new ParameterConverter();

        public ScenarioInterpreterForType(Type type,
                                          IEnumerable<MethodInfo> extensionMethodsForType,
                                          IInterpreterForTypeFactory factory)
        {
            _type = type;
            _factory = factory;

            DebugTrace.Trace(GetType().Name, "Building interpreter for: " + type);

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (MemberInfo member in GetMembers(flags)) 
                AddMatchers(member);

           

            foreach (var methodInfo in extensionMethodsForType)
            {
                DebugTrace.Trace(GetType().Name, "Added extension method matcher: " + methodInfo.Name);

                _memberMatchers.Add(new MethodNameMatcher(methodInfo));
            }
        }

        private MemberInfo[] GetMembers(BindingFlags flags)
        {
            var members = _type.GetMembers(flags);
           

            return FilterMembers(members);
        }

        private MemberInfo[] FilterMembers(MemberInfo[] members)
        {
            var ignore = new[] {"GetType", "ToString", "CompareTo", "GetTypeCode", "Equals", "GetHashCode"};
            return members.Where(m => !(m.MemberType == MemberTypes.Constructor || m.MemberType == MemberTypes.NestedType || ignore.Contains(m.Name))).ToArray();
        }

        private void AddMatchers(MemberInfo member)
        {
            _memberMatchers.Add(GetMemberMatcher(member));

            DebugTrace.Trace(GetType().Name, "Added reflection matcher: " + member.Name);

            AddRegexMatchersIfAttributePresent(member);
        }

        private static IMemberMatcher GetMemberMatcher(MemberInfo member)
        {
            if (member is MethodInfo)
                return new MethodNameMatcher((MethodInfo) member);
            
            return new PropertyOrFieldNameMatcher(member);
        }

        private void AddRegexMatchersIfAttributePresent(MemberInfo member)
        {
            var regexAttrs = member.GetCustomAttributes(typeof (ContextRegexAttribute), true);
            foreach (var regexAttr in regexAttrs.Cast<ContextRegexAttribute>())
            {
                DebugTrace.Trace(GetType().Name, "Added regex matcher: " + member.Name + ", \"" + regexAttr.Pattern + "\"");

                _memberMatchers.Add(new RegexMatcher(regexAttr.Pattern, member));
            }
        }

        public IEnumerable<InvocationChain> GetChains(string line)
        {
            DebugTrace.Trace(GetType().Name, "Interpreting '" + line + "' with type:" + _type.Name);
              
            var partialMatches = new List<PartialMatch>();
            foreach (var matcher in _memberMatchers)
            {
                foreach (var currentMatch in matcher.GetMatches(line) ?? new NameMatch[0])
                {
                    if (currentMatch is ExactMatch)
                    {
                        DebugTrace.Trace(GetType().Name, "Exact match");
              
                        yield return new InvocationChain
                                   {Invocations = new[] {BuildInvocation(matcher.MemberInfo, currentMatch)}};
                    }
                    else if (currentMatch is PartialMatch)
                    {
                        DebugTrace.Trace(GetType().Name, "Partial match -" + currentMatch.MatchedText);
                        partialMatches.Add((PartialMatch) currentMatch);
                    }
                }
            }

            var partialMatchChains = GetPartialMatchChains(line, partialMatches);
            foreach (var partialMatchChain in partialMatchChains)
                yield return partialMatchChain;
        }

        private IEnumerable<InvocationChain> GetPartialMatchChains(string line, IEnumerable<PartialMatch> partialMatches)
        {
            foreach (var partialMatch in partialMatches)
                foreach (InvocationChain chain in GetChainsFromPartialMatch(partialMatch, line))
                    if (chain != null)
                        yield return chain;
        }

        private IEnumerable<InvocationChain> GetChainsFromPartialMatch(PartialMatch partialMatch, string line)
        {
            var partialChain = new InvocationChain(BuildInvocation(partialMatch.MemberInfo, partialMatch));

            var matchedText = partialChain.Invocations.Last().MatchedText;
            var remainingLine = line.Substring(matchedText.Length).Trim();

            return TryToRecursivelyExtendPartialMatch(partialChain, remainingLine, partialMatch);
        }

        private static Invocation BuildInvocation(MemberInfo memberInfo, NameMatch currentMatch)
        {
            if (memberInfo is MethodInfo)
                return new Invocation(memberInfo, 
                    BuildParamValues((MethodInfo) memberInfo, currentMatch.ParamValues),
                    BuildRawParamValues((MethodInfo) memberInfo, currentMatch.ParamValues),
                                      currentMatch.MatchedText);

            return new Invocation(memberInfo, new string[0], new string[0], currentMatch.MatchedText);
        }

        private IEnumerable<InvocationChain> TryToRecursivelyExtendPartialMatch(InvocationChain chain, string remainingLine,
                                                                   PartialMatch partial)
        {
            if (partial.TerminatingType == null)
                yield break;
            var chainedMapper = _factory.GetInterpreterForType(partial.TerminatingType);
            foreach (var childChain in chainedMapper.GetChains(remainingLine))
            {               
                var union = chain.Invocations.Union(childChain.Invocations);

                yield return new InvocationChain(union.ToArray());
            }
        }

        private static IEnumerable<object> BuildParamValues(MethodBase member, Dictionary<string, object> paramValues)
        {
            var parameters = member.GetParameters();

            if (member.IsStatic)
                parameters = parameters.Skip(1).ToArray();

            foreach (ParameterInfo parameterInfo in parameters)
            {
                if (paramValues.ContainsKey(parameterInfo.Name))
                    yield return ConvertParam(paramValues[parameterInfo.Name].ToString(), parameterInfo.ParameterType);
                else
                    throw new ArgumentException("Could not resolve parameter " + parameterInfo.Name);
            }
        }

        private static IEnumerable<string> BuildRawParamValues(MethodBase member, Dictionary<string, object> paramValues)
        {
            var parameters = member.GetParameters();

            if (member.IsStatic)
                parameters = parameters.Skip(1).ToArray();

            foreach (ParameterInfo parameterInfo in parameters)
            {
                if (paramValues.ContainsKey(parameterInfo.Name))
                    yield return (string)paramValues[parameterInfo.Name];
            }
        }

        private static object ConvertParam(string s, Type t)
        {
            return _parameterConverter.Convert(s, t);
        }
    }
}