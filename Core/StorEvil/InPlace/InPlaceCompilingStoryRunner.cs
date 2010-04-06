using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public class InPlaceCompilingStoryRunner : InPlaceStoryRunnerBase
    {
        private readonly MemberInvoker _memberInvoker;
        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly AssemblyGenerator _generator = new AssemblyGenerator();

        public InPlaceCompilingStoryRunner(IResultListener listener,
                                           IScenarioPreprocessor preprocessor,
                                           ScenarioInterpreter scenarioInterpreter,
                                           IStoryFilter filter,
                                            IStoryContextFactory contextFactory)
            : base(listener, preprocessor, filter, contextFactory)
        {
            _scenarioInterpreter = scenarioInterpreter;
            _memberInvoker = new MemberInvoker();
        }

        protected override int Execute(Story story, IEnumerable<Scenario> scenarios, StoryContext context)
        {
            Scenario[] asArray = scenarios.ToArray();
            var assembly = _generator.GenerateAssembly(story, asArray);

            return ExecuteAssemblyDriver(assembly, story, context, asArray);
        }

        private int ExecuteAssemblyDriver(Assembly assembly, Story story, StoryContext context, Scenario[] scenarios)
        {
            var driverType = GetDriverType(assembly);
            var driver = Activator.CreateInstance(driverType,
                                                  ResultListener,
                                                  _memberInvoker,
                                                  _scenarioInterpreter,
                                                  scenarios
                );

            var methodInfo = driverType.GetMethod("Execute");

            return (int) methodInfo.Invoke(driver, new[] {context});
        }

        private static Type GetDriverType(Assembly assembly)
        {
            return assembly.GetTypes().First();
        }
    }
}