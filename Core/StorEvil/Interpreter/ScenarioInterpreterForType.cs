using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matches;
using StorEvil.Interpreter.ParameterConverters;

namespace StorEvil.Interpreter
{
    public class ScenarioInterpreterForType
    {
        private readonly IInterpreterForTypeFactory _factory;
        private readonly ParameterConverter _parameterConverter;
        private readonly ContextType _contextType;

        public ScenarioInterpreterForType(ContextType contextType,                                         
                                          IInterpreterForTypeFactory factory,
                                          ParameterConverter parameterConverter)
        {
            _contextType = contextType;         
            _factory = factory;
            _parameterConverter = parameterConverter;
        }      

        public IEnumerable<InvocationChain> GetChains(string line)
        {
            DebugTrace.Trace(GetType().Name, "Interpreting '" + line + "' with type:" + _contextType.WrappedType.Name);
              
            var partialMatches = new List<PartialMatch>();
            foreach (var matcher in _contextType.MemberMatchers)
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
            return partialMatches
                .SelectMany(pm => GetChainsFromPartialMatch(pm, line))
                .Where(x => x != null);
        }

        private IEnumerable<InvocationChain> GetChainsFromPartialMatch(PartialMatch partialMatch, string line)
        {
            var partialChain = new InvocationChain(BuildInvocation(partialMatch.MemberInfo, partialMatch));

            var matchedText = partialChain.Invocations.Last().MatchedText;
            var remainingLine = line.Substring(matchedText.Length).Trim();

            return TryToRecursivelyExtendPartialMatch(partialChain, remainingLine, partialMatch);
        }

        private Invocation BuildInvocation(MemberInfo memberInfo, NameMatch currentMatch)
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

        private IEnumerable<object> BuildParamValues(MethodBase member, Dictionary<string, object> paramValues)
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

        private IEnumerable<string> BuildRawParamValues(MethodBase member, Dictionary<string, object> paramValues)
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

        private object ConvertParam(string s, Type t)
        {
            return _parameterConverter.Convert(s, t);
        }
    }
}