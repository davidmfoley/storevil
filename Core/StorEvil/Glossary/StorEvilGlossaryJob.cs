using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funq;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Context.Matchers;
using StorEvil.Context.WordFilters;
using StorEvil.Interpreter;
using StorEvil.Utility;

namespace StorEvil.Core
{
    public class StorEvilGlossaryJob : IStorEvilJob
    {
        private readonly IStepProvider _stepProvider;
        private readonly IStepDescriber _stepDescriber;

        public StorEvilGlossaryJob(IStepProvider stepProvider, IStepDescriber stepDescriber)
        {
            _stepProvider = stepProvider;
            _stepDescriber = stepDescriber;
        }

        public int Run()
        {
            var descriptions = _stepProvider
                .GetSteps()
                .Select(x => _stepDescriber.Describe(x));

            foreach (var stepDescription in descriptions.OrderBy(x => x))
            {
               System.Console.WriteLine(stepDescription);                
            }
            return 0;
        }
    }

    public class StepProvider : IStepProvider
    {
        private readonly AssemblyRegistry _assemblyRegistry;
        private readonly ContextWrapperFactory _wrapperFactory;

        public StepProvider(AssemblyRegistry assemblyRegistry, ContextWrapperFactory wrapperFactory)
        {
            _assemblyRegistry = assemblyRegistry;
            _wrapperFactory = wrapperFactory;
        }

        public IEnumerable<StepDefinition> GetSteps()
        {
            foreach (var type in _assemblyRegistry.GetTypesWithCustomAttribute<ContextAttribute>())
            {
                var wrapper = _wrapperFactory.GetWrapper(type);
                foreach (var memberMatcher in wrapper.MemberMatchers)
                {
                    yield return BuildStepDefinition(type, memberMatcher);
                }
            }
        }

        private StepDefinition BuildStepDefinition(Type declaringType, IMemberMatcher memberMatcher)
        {
            return new StepDefinition { DeclaringType = declaringType, Matcher = memberMatcher };
        }
    }

    public interface IStepDescriber
    {
        string Describe(StepDefinition stepDefinition);
    }
    public interface IStepProvider
    {
        IEnumerable<StepDefinition> GetSteps();
    }

    public class StepDefinition
    {
        public Type DeclaringType { get; set; }

        public IMemberMatcher Matcher { get; set; }
    }


    public class GlossaryConfigurator : ContainerConfigurator<GlossarySettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, GlossarySettings customSettings)
        {
            container.EasyRegister<IStorEvilJob, StorEvilGlossaryJob>();
            container.EasyRegister<IStepDescriber, StepDescriber>();
            container.EasyRegister<IStepProvider, StepProvider>();
            container.EasyRegister<ContextWrapperFactory>();
        }

    }

    public class StepDescriber : IStepDescriber
    {
        public string Describe(StepDefinition stepDefinition)
        {
            if (stepDefinition.Matcher is MethodNameMatcher)
            {
                return DescribeMethodNameMatcher(stepDefinition.Matcher as MethodNameMatcher);
            }

            return null;
        }

        private string DescribeMethodNameMatcher(MethodNameMatcher methodNameMatcher)
        {
            var filters = methodNameMatcher.WordFilters;

            var filtersTranslated = filters.Select(x => TranslateWordFilter(x)).ToArray();

            return string.Join(" ", filtersTranslated);
        }

        private string TranslateWordFilter(WordFilter wordFilter)
        {
            if (wordFilter is TextMatchWordFilter)
            {
                return ((TextMatchWordFilter)wordFilter).Word;
            }
            else if (wordFilter is ParameterMatchWordFilter)
            {
                var paramMatch = ((ParameterMatchWordFilter)wordFilter);
                return string.Format("<{0} {1}>", TranslateTypeName(paramMatch.ParameterType), paramMatch.ParameterName);
            }

            return "";
        }

        private string TranslateTypeName(Type t)
        {
            if (t == typeof(int))
            {
                return "int";
            }
            if (t == typeof(double))
            {
                return "double";
            }
            if (t == typeof(string))
            {
                return "string";
            }
            if (t == typeof(bool))
            {
                return "bool";
            }
            return t.Name;
        }
    }

    public class GlossarySettings
    {
    }
}