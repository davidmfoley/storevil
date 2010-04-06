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
        private readonly AssemblyGenerator _generator;
        private int _failures;

        public InPlaceCompilingStoryRunner(IResultListener listener,
                                           IScenarioPreprocessor preprocessor,
                                           ScenarioInterpreter scenarioInterpreter,
                                           IStoryFilter filter,
                                            IStoryContextFactory contextFactory)
            : base(listener, preprocessor, filter, contextFactory)
        {
            _scenarioInterpreter = scenarioInterpreter;
            _memberInvoker = new MemberInvoker();
            _generator = new AssemblyGenerator(preprocessor);
        }

        protected override void Execute(Story story, IEnumerable<Scenario> scenarios, StoryContext context)
        {
            Scenario[] asArray = scenarios.ToArray();
            var assembly = _generator.GenerateAssembly(story, new string[0]);

            //ExecuteAssemblyDriver(assembly, context, asArray);
        }

        private void ExecuteAssemblyDriver(Assembly assembly, StoryContext context, Scenario[] scenarios)
        {
            var driverType = GetDriverType(assembly);
            var driver = Activator.CreateInstance(driverType,
                                                  ResultListener,
                                                  _memberInvoker,
                                                  _scenarioInterpreter,
                                                  scenarios
                );

            var methodInfo = driverType.GetMethod("Execute");

            _failures += (int) methodInfo.Invoke(driver, new[] {context});
        }

        private static Type GetDriverType(Assembly assembly)
        {
            return assembly.GetTypes().First();
        }
    }
}