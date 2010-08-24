using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using StorEvil.Context;
using StorEvil.Context.Matchers;
using StorEvil.Interpreter;

namespace StorEvil.Core
{
    public interface IStepProvider
    {
        IEnumerable<StepDefinition> GetSteps();
    }

    public class StepProvider : IStepProvider
    {
        private readonly AssemblyRegistry _assemblyRegistry;
        private readonly ContextTypeFactory _typeFactory;

        public StepProvider(AssemblyRegistry assemblyRegistry, ContextTypeFactory typeFactory)
        {
            _assemblyRegistry = assemblyRegistry;
            _typeFactory = typeFactory;
        }

        public IEnumerable<StepDefinition> GetSteps()
        {
            var typesWithCustomAttribute = _assemblyRegistry.GetTypesWithCustomAttribute<ContextAttribute>();
            return typesWithCustomAttribute.SelectMany(type => GetStepsForType(type, 2, FilterOutExtensionMethods));
        }

        private static bool FilterOutExtensionMethods(IMemberMatcher matcher)
        {
            var methodInfo = matcher.MemberInfo as MethodInfo;
            return (methodInfo == null) ||
                   (!methodInfo.IsDefined(typeof (ExtensionAttribute), true));
        }

        private IEnumerable<StepDefinition> GetStepsForType(Type type, int levels, Predicate<IMemberMatcher> include)
        {
            var wrapper = _typeFactory.GetWrapper(type);
            var matchers = wrapper.MemberMatchers.Where(x => include(x));
            return matchers.Select(memberMatcher => BuildStepDefinition(type, memberMatcher, levels));
        }

        private StepDefinition BuildStepDefinition(Type type, IMemberMatcher memberMatcher, int levels)
        {
            var stepDefinition = new StepDefinition { DeclaringType = type, Matcher = memberMatcher };
                    
            if (levels > 0 && memberMatcher.ReturnType != null && memberMatcher.ReturnType != typeof (void))
                stepDefinition.Children = GetStepsForType(memberMatcher.ReturnType, levels - 1, x=>true);
            else
                stepDefinition.Children = new StepDefinition[0];

            return stepDefinition;
        }
    }
}